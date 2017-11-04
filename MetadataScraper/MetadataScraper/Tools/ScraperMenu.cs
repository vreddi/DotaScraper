using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataScraper
{
    public class ScraperMenu
    {
        /// <summary>
        /// 
        /// </summary>
        public static void ShowMenu() {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Commands:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1) Parse all hero data");
            Console.WriteLine("2) Generate Luis hero data entries");
            Console.WriteLine("3) Parse all item data");
            Console.WriteLine("4) [NEW] Parse all hero data");
            Console.WriteLine("5) Open Skill Scraping Menu");
            Console.WriteLine("0) Exit");
            Console.ResetColor();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowSkillMenu() {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Commands:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1) Scrape all skills");
            Console.WriteLine("2) Scrape all skills using heros directory cache");
            Console.WriteLine("3) Scrape skills for a single hero");
            Console.WriteLine("4) Back");
            Console.ResetColor();
        }
    }
}
