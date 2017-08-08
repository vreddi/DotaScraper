using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataScraper
{
    public enum StatValueType {
        Number,
        Percentage
    }

    public enum StatType {
        Attribute,
        Effect
    }

    public class Stat
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public StatType StatType { get; set; }

        public StatValueType ValueType { get; set; }
    }
}
