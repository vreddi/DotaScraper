using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataScraper
{
    public class ScraperMenu
    {

        public static void ShowMenu() {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Commands:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1) Parse all hero data");
            Console.WriteLine("2) Generate Luis hero data entries");
            Console.WriteLine("3) Parse all item data");
            Console.ResetColor();
        }
    }
}
