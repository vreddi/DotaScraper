using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MetadataScraper
{
    public class Program
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        private static readonly string HeroesMetadataDirectory = @"C:\Repos\DotaBot\Metadata\Heroes\";

        private static readonly string HeroesMetadataHeroFormat = @"C:\Repos\DotaBot\Metadata\Heroes\{0}.json";

        private static readonly string LuisHeroEntriesPath = @"C:\Repos\DotaBot\Metadata\Luis\HeroEntries.json";

        public static void Main(string[] args)
        {
            var success = AsyncMain().Result;
        }

        public static async Task<bool> AsyncMain()
        {
            Console.WriteLine("Commands");
            Console.WriteLine("1) Parse all hero data");
            Console.WriteLine("2) Generate Luis hero data entries");
            Console.WriteLine("3) Parse all item data");

            var cmd = Console.ReadLine();

            switch (cmd)
            {
                case "1":
                    Console.WriteLine("Refresh hero data from DotaBuff? (y/n): ");
                    cmd = Console.ReadLine();

                    var refreshData = cmd.Contains("y");
                    await ParseHeroData(refreshData);
                    break;

                case "2":
                    await CreateLuisHeroData();
                    break;

                case "3":
                    await ParseItemData();
                    break;

                default:
                    Console.WriteLine("Could not find anything");
                    break;
            }

            return true;
        }

        private static async Task CreateLuisHeroData()
        {
            var heroes = await GetOpenDotaHeroes();

            var luisList = new List<LuisEntry>();

            foreach (var hero in heroes)
            {
                var entry = new LuisEntry { canonicalForm = hero.name, list = { hero.localized_name } };
                if (hero.aliases != null)
                {
                    entry.list.AddRange(hero.aliases);
                }

                luisList.Add(entry);
            }

            File.WriteAllText(LuisHeroEntriesPath,
                JsonConvert.SerializeObject(luisList, jsonSerializerSettings));
        }

        private static async Task ParseHeroData(bool overrideOldData = false)
        {
            var heroes = await GetOpenDotaHeroes(overrideOldData);

            WriteHeroFiles(heroes);
        }

        private static async Task ParseItemData(bool overrideOldData = false)
        {
            var items = await Scraper.GetAllItems();

            foreach (var item in items) {
                Console.WriteLine(item.Name + " processed.");
            }
            Console.ReadLine();
            //WriteHeroFiles(heroes);
        }

        private static void WriteHeroFiles(List<OpenDotaHero> heroes)
        {
            foreach (var hero in heroes)
            {
                File.WriteAllText(string.Format(HeroesMetadataHeroFormat, hero.name),
                    JsonConvert.SerializeObject(hero, jsonSerializerSettings));
            }
        }

        private static async Task<List<OpenDotaHero>> GetOpenDotaHeroes(bool overrideOldData = false)
        {
            var heroes = new List<OpenDotaHero>();
            var files = Directory.GetFiles(HeroesMetadataDirectory);

            // File exists for all heroes
            if (files.Length >= 113 && !overrideOldData)
            {
                heroes = GetLocalHeroData(files);
            }
            else
            {
                var oldHeroes = GetLocalHeroData(files);
                heroes = await Scraper.GetAllHeroes(oldHeroes);
            }

            return heroes;
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
