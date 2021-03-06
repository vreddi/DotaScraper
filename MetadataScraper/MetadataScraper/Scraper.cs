﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace MetadataScraper
{
    public static class Scraper
    {
        static HttpClient Client = new HttpClient();

        const string OpenDotaEndpoint = "https://api.opendota.com/";

        const string DotabuffEndpoint = "https://dotabuff.com/";

        const string DotabuffHeroesPathFormat = "/heroes/{0}";

        private const string DotabuffHeroSkillsClass = "hero-secondary-ability-icons";

        public static List<Talent> ParseTalents(HtmlDocument doc)
        {
            var talents = new List<Talent>();
            var talentRows = doc.DocumentNode.Descendants("tr")
                .Where(d => d.GetAttributeValue("class", "").Contains("talent-data-row"));
            foreach (var talentRow in talentRows)
            {
                var talent = new Talent();
                talent.Level = int.Parse(talentRow.FirstChild.FirstChild.InnerHtml);

                var talentOptions = talentRow.Descendants("div")
                    .Where(d => d.GetAttributeValue("class", "").Contains("talent-name"));
                talent.Options.AddRange(talentOptions.Select(t => t.InnerHtml));

                talents.Add(talent);
            }

            return talents;
        }

        public static async Task<List<Item>> GetAllItems()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            int itemRank = 1;
            var htmlWeb = new HtmlWeb();
            var dotabuffDoc = htmlWeb.Load(DotabuffEndpoint + "items?date=week");
            List<Item> items = new List<Item>();

            var query = from table in dotabuffDoc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                        from body in table.SelectNodes("tbody").Cast<HtmlNode>()
                        from row in body.SelectNodes("tr").Cast<HtmlNode>()
                        select new { ItemRow = row };

            foreach (var element in query)
            {
                var ItemRowCells = element.ItemRow.SelectNodes("td");
                var item = new Item();
                ItemShopAvailability shops = new ItemShopAvailability();

                item.PopularityRank = itemRank;
                item.Name = ItemRowCells.ElementAt<HtmlNode>(0).GetAttributeValue("data-value", null);
                item.LocalizedName = Item.ConvertItemNameToLocalizedName(item.Name);
                item.SourceLink = DotabuffEndpoint + "items/" + item.LocalizedName;
                item.TimesUsed = Convert.ToInt64(ItemRowCells.ElementAt<HtmlNode>(2).GetAttributeValue("data-value", null));
                item.UseRate = Convert.ToDouble(ItemRowCells.ElementAt<HtmlNode>(3).GetAttributeValue("data-value", null));
                item.WinRate = Convert.ToDouble(ItemRowCells.ElementAt<HtmlNode>(4).GetAttributeValue("data-value", null));

                item.Stats = new List<Stat>();

                Console.WriteLine("Parsing " + item.Name + "...");

                var statNodes = new List<HtmlNode>();

                var dotaBuffItemDoc = htmlWeb.Load(item.SourceLink);

                bool docStatsExists = dotaBuffItemDoc.DocumentNode.SelectNodes("//article")[0].SelectNodes("//div[@class = 'stats']") != null;
                bool shopInfoExists = dotaBuffItemDoc.DocumentNode.SelectNodes("//article")[0].SelectNodes("//div[@class = 'shop']") != null;

                IEnumerable<HtmlNode> stats = new List<HtmlNode>();
                IEnumerable<HtmlNode> shopNodes = new List<HtmlNode>();

                if (docStatsExists) {
                    stats = from article in dotaBuffItemDoc.DocumentNode.SelectNodes("//div[@class = 'portable-show-item-details-default']").Cast<HtmlNode>()
                            from statDiv in article.SelectNodes("//div[@class = 'stats']")
                            from stat in statDiv.SelectNodes("div[contains(@class, 'stat')]")
                            select stat;
                }

                IEnumerable<HtmlNode> descriptions = from article in dotaBuffItemDoc.DocumentNode.SelectNodes("//div[@class = 'portable-show-item-details-default']").Cast<HtmlNode>()
                                                     from description in article.SelectNodes("//div[@class = 'description']").Cast<HtmlNode>()
                                                     select description;

                // Get Shop Information
                if (shopInfoExists) {
                    shopNodes = from article in dotaBuffItemDoc.DocumentNode.SelectNodes("//div[@class = 'portable-show-item-details-default']").Cast<HtmlNode>()
                                from shop in article.SelectNodes("//div[@class = 'shop']").Cast<HtmlNode>()
                                select shop;

                    foreach (var shopNode in shopNodes)
                    {
                        shops.DetermineShopBasedOnText(shopNode.InnerText);
                    }

                    item.Shops = shops;
                }

                // Get Build From
                HtmlNode buildsFrom = dotaBuffItemDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'item-builds-from')]");

                if(buildsFrom != null) {
                    item.BuiltFrom = new List<string>();
                    IEnumerable<HtmlNode> itemImgs = buildsFrom.SelectNodes(".//img");

                    foreach (HtmlNode itemImg in itemImgs) {

                        if (itemImg != null) {
                            item.BuiltFrom.Add(Item.ConvertItemNameToLocalizedName(
                                itemImg.GetAttributeValue("alt", "")
                            ));
                        }
                    }                
                }

                // Get Build Into
                HtmlNode buildsInto = dotaBuffItemDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'item-builds-into')]");

                if (buildsInto != null)
                {
                    item.BuildInto = new List<string>();
                    IEnumerable<HtmlNode> itemImgs = buildsInto.SelectNodes(".//img");

                    foreach (HtmlNode itemImg in itemImgs)
                    {

                        if (itemImg != null)
                        {
                            item.BuildInto.Add(Item.ConvertItemNameToLocalizedName(
                                itemImg.GetAttributeValue("alt", "")
                            ));
                        }
                    }
                }

                // Get Notes
                HtmlNode notes = dotaBuffItemDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'notes')]");

                // Populate notes
                if (notes != null) {
                    IEnumerable<HtmlNode> notes_p = notes.SelectNodes("p");

                    item.Notes = new List<string>();

                    foreach (HtmlNode p in notes_p) {
                        item.Notes.Add(p.InnerText.Replace("&#39;", "'"));
                    }
                }

                HtmlNode toolTipHeader = dotaBuffItemDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'tooltip-header')]");

                string cost = toolTipHeader.SelectSingleNode("//span[@class = 'value']").InnerText.Replace(",", "");

                if (cost != null) {
                    try {
                        item.Cost = Convert.ToInt16(cost);
                    }
                    catch (Exception ex) {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Error: " + ex);
                        Console.ResetColor();
                    }
                }

                // Populate Stats
                foreach (var stat in stats) {

                    var itemStat = new Stat();
                    var docValue = stat.SelectSingleNode("span[@class = 'value']").InnerText;
                    itemStat.Name = stat.SelectSingleNode("span[@class = 'label']").InnerText;

                    string statDivClasses = stat.GetAttributeValue("class", null);

                    if (statDivClasses != null) {
                        if (statDivClasses.Contains("attribute")){
                            itemStat.StatType = StatType.Attribute;
                        }
                        else if (statDivClasses.Contains("effect")) {
                            itemStat.StatType = StatType.Effect;
                        }
                    }

                    if (docValue.Contains("%"))
                    {
                        itemStat.ValueType = StatValueType.Percentage;
                        itemStat.Value = Convert.ToInt16(docValue.Replace("%", ""));
                    }
                    else {
                        itemStat.ValueType = StatValueType.Number;
                        itemStat.Value = Convert.ToDouble(docValue.Replace(",", ""));
                    }

                    item.Stats.Add(itemStat);
                }

                // Populate descriptions
                item.Descriptions = new List<ItemAbility>();
                foreach (var childNode in descriptions.First().ChildNodes) {
                    string classAttrValue = childNode.GetAttributeValue("class", null);

                    if (classAttrValue != null) {
                        ItemAbility ability = new ItemAbility();
                        switch (classAttrValue) {
                            case "description-block passive":
                                ability.Type = ItemAbilityType.Passive;
                                ability.Name = childNode.SelectSingleNode("div[@class = 'description-block-header']").InnerText.Replace("Passive: ", "");
                                ability.Description = childNode.InnerText.Replace("Passive: " + ability.Name, "");
                                item.Descriptions.Add(ability);
                                break;

                            case "description-block active":
                                ability.Type = ItemAbilityType.Active;
                                ability.Name = childNode.SelectSingleNode("div[@class = 'description-block-header']").InnerText.Replace("Active: ", "");
                                ability.Description = childNode.InnerText.Replace("Active: " + ability.Name, "");
                                item.Descriptions.Add(ability);
                                break;

                            case "description-block use":
                                ability.Type = ItemAbilityType.Use;
                                ability.Name = childNode.SelectSingleNode("div[@class = 'description-block-header']").InnerText.Replace("Use: ", "");
                                ability.Description = childNode.InnerText.Replace("Use: " + ability.Name, "");
                                item.Descriptions.Add(ability);
                                break;

                            case "cooldown-and-cost":
                                HtmlNode coolDownDiv = childNode.SelectSingleNode("div[@class = 'cooldown']");
                                HtmlNode manaCostDiv = childNode.SelectSingleNode("div[@class = 'manacost']");

                                if (coolDownDiv != null) {

                                    HtmlNodeCollection spanValues = coolDownDiv.SelectSingleNode("span[@class = 'value']").SelectNodes("span");

                                    if (spanValues != null) {
                                        item.Descriptions.First().CoolDown = new List<double>();

                                        foreach (HtmlNode spanValue in spanValues)
                                        {
                                            item.Descriptions.First().CoolDown.Add(Convert.ToDouble(spanValue.InnerText));
                                        }

                                    }                                  
                                }

                                if (manaCostDiv != null)
                                {
                                    item.Descriptions.First().ManaCost = Convert.ToInt16(manaCostDiv.SelectSingleNode("span[@class = 'value']").InnerText);
                                }
                                break;
                        }
                    }
                }

                var loreElement = dotaBuffItemDoc.DocumentNode.SelectSingleNode("//div[@class = 'lore']");

                if (loreElement != null) {
                    item.Lore = loreElement.InnerText;
                }

                items.Add(item);
                itemRank++;

                // Show progress bar and add some wait between querying each item document
                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i <= 100; i++)
                    {
                        progress.Report((double)i / 100);
                        Thread.Sleep(20);
                    }
                }
                Console.WriteLine("Done.");
            }

            return items;
        }
    }
}
