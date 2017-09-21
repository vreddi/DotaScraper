using System;
using System.Collections.Generic;

namespace MetadataScraper
{
    [Serializable]
    public class Item
    {
        public int PopularityRank { get; set; }

        public string SourceLink { get; set; }

        public string LocalizedName { get; set; }

        public string Name { get; set; }

        public string Lore { get; set; }

        public List<ItemAbility> Descriptions { get; set; }

        public List<string> Notes { get; set; }

        public long TimesUsed { get; set; }

        public double UseRate { get; set; }

        public double WinRate { get; set; }

        public List<Stat> Stats { get; set; }

        public int Cost { get; set; }

        public string Image { get; set; }

        public ItemShopAvailability Shops { get; set; }

        public List<string> Aliases { get; set; }

        public List<string> BuiltFrom { get; set; }

        public List<string> BuildInto { get; set; }


        /// <summary>
        /// Given the name of the item, this converts and returns the localized-name of the
        /// item.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ConvertItemNameToLocalizedName(string name)
        {
            return name.ToLower().Replace(" ", "-").Replace("'", "").Replace("(", "").Replace(")", "").Replace("&#39;", "");
        }
    }
}
