using System;
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
            string cmd = null;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            while (cmd != "0") {

                cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "1":
                        if (Program.LocalHeroCache.Count != 0)
                        {
                            Console.WriteLine("Refresh hero data from DotaBuff? (y/n): ");
                            cmd = Console.ReadLine();
                        }

                        var refreshData = cmd.Contains("y");
                        //await ParseHeroData(refreshData);
                        break;

                    case "2":
                        break;

                    case "3":
                        await ParseItemData();
                        break;

                    case "4":
                        ParseHeroDataNew();
                        break;

                    case "5":
                        string cmd2 = null;
                        ScraperMenu.ShowSkillMenu();
                        while (cmd2 != "4") {
                            cmd2 = HandleSkillScrapingMenuControl(cmd2);
                        }
                        ScraperMenu.ShowMenu();
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        private static string HandleSkillScrapingMenuControl(string cmd) {

            cmd = Console.ReadLine();

            switch (cmd) {

                case "1":
                    Console.WriteLine("Feature Currently unavailable.");
                    Console.WriteLine("Enter any key to continue...");
                    Console.ReadLine();
                    ScraperMenu.ShowSkillMenu();
                    break;

                case "2":
                    Console.WriteLine("Feature Currently unavailable.");
                    Console.WriteLine("Enter any key to continue...");
                    Console.ReadLine();
                    ScraperMenu.ShowSkillMenu();
                    break;

                case "3":
                    Console.WriteLine("Enter Name of hero (localized)");
                    string heroName = Console.ReadLine();
                    Program.ParseSkillData(heroName);
                    Console.WriteLine("Enter any key to continue...");
                    Console.ReadLine();
                    ScraperMenu.ShowSkillMenu();
                    break;

                case "4":
                    break;

                default:
                    Console.WriteLine("Could not find anything");
                    break;
            }

            return cmd;
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

        private static void ParseSkillData(string heroLocalizedName = null)
        {
            List<Skill> skills = new List<Skill>();

            if (heroLocalizedName != null) {
                skills = SkillScraper.ScrapeHeroSkills(heroLocalizedName);
            }

            // TODO: Handle other skill scraping menu cases
 
            foreach (var skill in skills)
            {
                Console.WriteLine(skill.Name + " processed.");
            }

            WriteSkillFiles(skills);
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

        private static void WriteSkillFiles(List<Skill> skills) {
            foreach (var skill in skills) {
                File.WriteAllText(string.Format(SkillsMetadataSkillFormat, skill.Name),
                        JsonConvert.SerializeObject(skill, jsonSerializerSettings)
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
