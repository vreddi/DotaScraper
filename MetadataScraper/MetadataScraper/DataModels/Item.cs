using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataScraper
{
    public class Item
    {
        public int PopularityRank { get; set; }

        public long TimesUsed { get; set; }

        public double UseRate { get; set; }

        public double WinRate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Cost { get; set; }

        public string Lore { get; set; }

        public string Image { get; set; }

        public List<string> Aliases { get; set; }

        public List<Skill> Skills { get; set; }

    }
}
