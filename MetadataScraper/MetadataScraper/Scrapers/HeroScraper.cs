using System;
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
    public static class HeroScraper
    {
        private static HttpClient Client = new HttpClient();

        // Dotabuff relevance
        private const string DotabuffHerosEndpoint = "https://www.dotabuff.com/heroes";
        private const string DotabuffHeroGridCssClass = "hero-grid";
        private const string DotabuffHeroHeaderContentSecondaryClass = "header-content-secondary";

        // Gamepedia Relevance
        private const string GamepediaHeroInfoBoxClass = "infobox";
        private const string GamepediaHeroEvenRowsGray = "evenrowsgray";
        private const string GamepediaHeroOddRowsGray = "oddrowsgray";


        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of heros</returns>
        public static List<Hero> ParseAllHeros() {

            var htmlWeb = new HtmlWeb();
            var dotaBuffHerosDoc = htmlWeb.Load(DotabuffHerosEndpoint);
            List<Hero> heros = new List<Hero>();

            ScraperConsole.LogInfo(string.Format("Loading document {0}", DotabuffHerosEndpoint));
            HtmlNode heroGrid = dotaBuffHerosDoc.DocumentNode.SelectSingleNode(
                    string.Format(".//div[@class = '{0}']", DotabuffHeroGridCssClass
                )
            );

            if (heroGrid != null)
            {
                IEnumerable<HtmlNode> heroLinks = heroGrid.SelectNodes(".//a");
                foreach (HtmlNode link in heroLinks) {
                    try
                    {
                        string path = link.GetAttributeValue("href", null);
                        string heroLocalizedName = path.Replace("/heroes/", "");
                        string dotabuffPath = Program.DotaBuffEndpoint + path;

                        Hero hero = new MetadataScraper.Hero();

                        // Populate Hero Name Info
                        hero.Name = link.SelectSingleNode(".//div[@class = 'name']").InnerText;
                        hero.LocalizedName = heroLocalizedName;

                        // Free a line after each hero parse
                        Console.WriteLine();

                        ScraperConsole.LogKeyInfo("Parsing data for hero: ", hero.Name);

                        HeroScraper.GetDotabuffHero(dotabuffPath, hero);
                        ScraperConsole.Log("Grabbing information from " + Program.DotaBuffEndpoint);

                        // Wait for 7s between each hero parse
                        // Have to respect:
                        // https://dotabuff.com/robots.txt
                        ProgressBar.StartProgressBar(7000);

                        string gamepediaPath = Program.GamepediaEndpoint + "/" + hero.Name.Replace(" ", "_");

                        HeroScraper.GetGamepediaHero(gamepediaPath, hero);
                        ScraperConsole.Log("Grabbing information from " + Program.GamepediaEndpoint);
                        
                        // Wait for 7s between each hero parse
                        // Have to respect:
                        // https://dota2.gamepedia.com/robots.txt
                        ProgressBar.StartProgressBar(7000);
                        heros.Add(hero);
                    }
                    catch (Exception error) {
                        ScraperConsole.LogError(error.ToString());
                    } 
                }

            }
            else {
                ScraperConsole.LogError(
                    string.Format(
                        "Unable to find '{0}' class under the document {1}", 
                        DotabuffHeroGridCssClass,
                        DotabuffHerosEndpoint
                    ),
                    ScraperConsole.ExceptionType.NotFound
                );
            }

            return heros;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hero"></param>
        /// <returns></returns>
        public static Hero GetDotabuffHero(string path, Hero hero = null) {

            var htmlWeb = new HtmlWeb();
            var dotaBuffHeroDocument = htmlWeb.Load(path);

            if (hero == null)
            {
                hero = new Hero();
            }

            IEnumerable<HtmlNode> headerContentSecondaryDl = dotaBuffHeroDocument.DocumentNode.SelectSingleNode(
                    string.Format(".//div[@class = '{0}']", DotabuffHeroHeaderContentSecondaryClass)
                ).SelectNodes(".//dd");

            hero.SourceLink = path;
            hero.Popularity = Convert.ToInt16(headerContentSecondaryDl.First().InnerText.Replace("th", ""));
            hero.WinRate = Convert.ToSingle(headerContentSecondaryDl.Last().InnerText.Replace("%", ""));

            HeroScraper.ParseDotabuffHeroLanePresence(dotaBuffHeroDocument, hero);

            return hero;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dotabuffHeroDocument"></param>
        /// <param name="hero"></param>
        /// <returns></returns>
        private static Hero ParseDotabuffHeroLanePresence(HtmlDocument dotabuffHeroDocument, Hero hero = null) {

            if (hero == null)
            {
                hero = new Hero();
            }

            IEnumerable<HtmlNode> sections = dotabuffHeroDocument.DocumentNode.SelectNodes(".//section");

            foreach (HtmlNode section in sections) {
                if (section.SelectSingleNode(".//header").InnerText.ToLower() == "lane presence") {
                    HtmlNode tbody = section.SelectSingleNode(".//tbody");
                    IEnumerable<HtmlNode> tableRows = tbody.SelectNodes(".//tr");

                    hero.LanePresence = new List<LanePresenceStats>();

                    foreach (HtmlNode tr in tableRows) {
                        HtmlNode[] tds = tr.SelectNodes(".//td").ToArray();
                        LaneType? lane = LanePresenceStats.GetLaneTypeFromString(tds[0].InnerText);

                        // If we are unable to resolve to a valid lane, then we skill this data
                        if (lane == null) {
                            continue;
                        }

                        LanePresenceStats heroLanePresence = new LanePresenceStats();

                        // Populate Lane Presence Data
                        heroLanePresence.Lane = (LaneType)lane;
                        heroLanePresence.Presence = Convert.ToSingle(tds[1].GetAttributeValue("data-value", null));
                        heroLanePresence.WinRate = Convert.ToSingle(tds[2].GetAttributeValue("data-value", null));
                        heroLanePresence.KdaRatio = Convert.ToSingle(tds[3].GetAttributeValue("data-value", null));
                        heroLanePresence.Gpm = Convert.ToSingle(tds[4].GetAttributeValue("data-value", null));
                        heroLanePresence.Xpm = Convert.ToSingle(tds[5].GetAttributeValue("data-value", null));

                        hero.LanePresence.Add(heroLanePresence);
                    }

                    break;
                }
            }

            return hero;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hero"></param>
        /// <returns></returns>
        public static Hero GetGamepediaHero(string path, Hero hero = null) {

            var htmlWeb = new HtmlWeb();
            var gamepediaHeroDocument = htmlWeb.Load(path);

            if (hero == null) {
                hero = new Hero();
            }

            HtmlNode heroInfobBox = gamepediaHeroDocument.DocumentNode.SelectSingleNode(
                    string.Format(".//table[@class = '{0}']", GamepediaHeroInfoBoxClass)
                );

            HtmlNode heroStatsSet1 = heroInfobBox.SelectSingleNode(
                    string.Format(".//table[@class = '{0}']", GamepediaHeroEvenRowsGray)
                );

            HtmlNode heroStatsSet2 = heroInfobBox.SelectSingleNode(
                    string.Format(".//table[@class = '{0}']", GamepediaHeroOddRowsGray)
                );

            HtmlNode a = heroStatsSet1.SelectSingleNode(".//tbody");

            // Parse Stat Set 1
            HtmlNode[] statSet1Rows = heroStatsSet1.SelectNodes(".//tr").ToArray();

            HeroDistributedStats<float>[] statSet1FillOrder = { 
                    hero.Health,
                    hero.HealthRegen,
                    hero.Mana,
                    hero.ManaRegen,
                    null,
                    hero.Armor,
                    hero.SpellDamage,
                    hero.AttacksPerSecond
                };

            for (int i = 1; i < statSet1Rows.Length; ++i) {
                HtmlNode[] tds = statSet1Rows[i].SelectNodes(".//td").ToArray();

                // Have to handle hero.Damage separately
                // TODO: Handle this case in a better way
                if (i == 5) {
                    string[] damageRange = tds[0].InnerText.Replace("%", "").Split('‒');
                    hero.Damage = new HeroDistributedStats<Range<float>>();

                    hero.Damage.Base = new Range<float>();
                    hero.Damage.Base.Min = Convert.ToSingle(damageRange[0]);
                    hero.Damage.Base.Max = Convert.ToSingle(damageRange[1]);

                    hero.Damage.Level1 = new Range<float>();
                    damageRange = tds[1].InnerText.Replace("%", "").Split('‒');
                    hero.Damage.Level1.Min = Convert.ToSingle(damageRange[0]);
                    hero.Damage.Level1.Max = Convert.ToSingle(damageRange[1]);

                    hero.Damage.Level15 = new Range<float>();
                    damageRange = tds[2].InnerText.Replace("%", "").Split('‒');
                    hero.Damage.Level15.Min = Convert.ToSingle(damageRange[0]);
                    hero.Damage.Level15.Max = Convert.ToSingle(damageRange[1]);

                    hero.Damage.Level25 = new Range<float>();
                    damageRange = tds[3].InnerText.Replace("%", "").Split('‒');
                    hero.Damage.Level25.Min = Convert.ToSingle(damageRange[0]);
                    hero.Damage.Level25.Max = Convert.ToSingle(damageRange[1]);
                }
                else {
                    statSet1FillOrder[i - 1].Base = Convert.ToSingle(tds[0].InnerText.Replace("%", ""));
                    statSet1FillOrder[i - 1].Level1 = Convert.ToSingle(tds[1].InnerText.Replace("%", ""));
                    statSet1FillOrder[i - 1].Level15 = Convert.ToSingle(tds[2].InnerText.Replace("%", ""));
                    statSet1FillOrder[i - 1].Level25 = Convert.ToSingle(tds[3].InnerText.Replace("%", ""));
                }
            }


            // Parse Stat Set 2
            HtmlNode[] statSet2Rows = heroStatsSet2.SelectNodes(".//tr").ToArray();

            hero.MovementSpeed = Convert.ToInt16(statSet2Rows[0].SelectSingleNode(".//td").InnerText);
            hero.TurnRate = Convert.ToSingle(statSet2Rows[1].SelectSingleNode(".//td").InnerText);

            hero.VisionRange = new HeroVisionRange();
            hero.VisionRange.Day = Convert.ToInt16(statSet2Rows[2].SelectSingleNode(".//td").InnerText.Split('/')[0]);
            hero.VisionRange.Night = Convert.ToInt16(statSet2Rows[2].SelectSingleNode(".//td").InnerText.Split('/')[1]);

            hero.AttackRange = Convert.ToInt16(statSet2Rows[3].SelectSingleNode(".//td").InnerText);

            // Projectile Speed is left out for now

            hero.AttackAnimation = new HeroAttackAnimation();
            hero.AttackAnimation.AttackPoint = Convert.ToSingle(statSet2Rows[5].SelectSingleNode(".//td").InnerText.Split('+')[0]);
            hero.AttackAnimation.AttackBackSwing = Convert.ToSingle(statSet2Rows[5].SelectSingleNode(".//td").InnerText.Split('+')[1]);

            hero.BaseAttackTime = Convert.ToSingle(statSet2Rows[6].SelectSingleNode(".//td").InnerText);
            hero.MagicResitance = Convert.ToSingle(statSet2Rows[7].SelectSingleNode(".//td").InnerText.Replace("%", ""));
            hero.CollisionSize = Convert.ToSingle(statSet2Rows[8].SelectSingleNode(".//td").InnerText);
            hero.Legs = Convert.ToInt16(statSet2Rows[9].SelectSingleNode(".//td").InnerText);

            return hero;
        }
    }
}