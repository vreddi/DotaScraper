﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Serialization;
using System.Threading;

namespace MetadataScraper
{
    public class Program
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        private static readonly string HeroesMetadataDirectory = @"D:\DotaEntities\Heros\";

        private static readonly string HeroesMetadataHeroFormat = @"D:\DotaEntities\Heros\{0}.json";

        private static readonly string SkillsMetadataDirectory = @"D:\DotaEntities\Skills\";

        private static readonly string SkillsMetadataSkillFormat = @"D:\DotaEntities\Skills\{0}.json";

        private static readonly string LuisHeroEntriesPath = @"C:\Repos\DotaBot\Metadata\Luis\HeroEntries.json";

        private static readonly string ItemsMetadataItemDirectory = @"D:\Github\Dotabot\Metadata\Items";

        private static readonly string ItemsMetadataItemFormat = @"D:\Github\Dotabot\Metadata\Items\{0}.json";

        public static readonly string DotaBuffEndpoint = "https://www.dotabuff.com";

        public static readonly string GamepediaEndpoint = "https://dota2.gamepedia.com";

        public static List<string> LocalHeroCache = new List<string>();

        public static void Main(string[] args)
        {
            ScraperMenu.ShowMenu();
             var success = AsyncMain().Result;
        }

        public static async Task<bool> AsyncMain()
        {
            ScraperMenu.ShowMenu();

            var cmd = Console.ReadLine();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            while (cmd != "0") {
                switch (cmd)
                {
                    case "1":
                        if (Program.LocalHeroCache.Count != 0)
                        {
                            Console.WriteLine("Refresh hero data from DotaBuff? (y/n): ");
                            cmd = Console.ReadLine();
                        }

                        var refreshData = cmd.Contains("y");
                        await ParseHeroData(refreshData);
                        break;

                    case "2":
                        break;

                    case "3":
                        await ParseItemData();
                        break;

                    case "4":
                        ParseHeroDataNew();
                        break;

                    case "0":
                        break;

                    default:
                        Console.WriteLine("Could not find anything");
                        break;
                }
            }         

            Console.WriteLine("Enter any key to exit...");
            Console.ReadLine();
            return true;
        }


        private static async Task ParseItemData(bool overrideOldData = false)
        {
            var items = await Scraper.GetAllItems();

            foreach (var item in items) {
                Console.WriteLine(item.Name + " processed.");
                Console.Write(item.SourceLink);
            }

            WriteItemFiles(items);
        }

        private static void ParseHeroDataNew()
        {
            var heros = HeroScraper.ParseAllHeros();

            foreach (var hero in heros)
            {
                Program.LocalHeroCache.Add(hero.LocalizedName);
                Console.WriteLine(hero.Name + " processed.");
                Console.Write(hero.SourceLink);
            }

            WriteNewHeroFiles(heros);
        }

        private static void WriteHeroFiles(List<OpenDotaHero> heroes)
        {
            foreach (var hero in heroes)
            {
                File.WriteAllText(string.Format(HeroesMetadataHeroFormat, hero.name),
                    JsonConvert.SerializeObject(hero, jsonSerializerSettings));
            }
        }

        private static void WriteItemFiles(List<Item> items) {
            foreach (var item in items) {
                File.WriteAllText(string.Format(ItemsMetadataItemFormat, item.LocalizedName),
                        JsonConvert.SerializeObject(item, jsonSerializerSettings)
                    );
            }
        }

        private static void WriteNewHeroFiles(List<Hero> heros)
        {
            foreach (var hero in heros)
            {
                File.WriteAllText(string.Format(HeroesMetadataHeroFormat, hero.LocalizedName),
                        JsonConvert.SerializeObject(hero, jsonSerializerSettings)
                    );
            }
        }

        private static List<OpenDotaHero> GetLocalHeroData(string[] files)
        {
            var heroes = new List<OpenDotaHero>();
            foreach (var file in files)
            {
                Console.WriteLine("Parsing data from " + file);
                var fileContent = File.ReadAllText(file);
                heroes.Add(JsonConvert.DeserializeObject<OpenDotaHero>(fileContent));
            }

            return heroes;
        }
    }
}
