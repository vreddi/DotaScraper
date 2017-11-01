using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataScraper
{
    public enum DamageType {
        Physical,
        Magical
    }

    public enum SkillAbilityType {
        Passive,
        PointTarget,
        UnitTarget
    }

    public enum AffectsType {
        EnemyUnits
    }

    [Serializable]
    public class Skill
    {
        public Skill()
        {
            this.Effects = new List<KeyValuePair<string, string>>();
            this.Cooldown = new List<double>();
            this.Stats = new List<StatEffect>();
            this.ManaCost = new List<int>();
        }

        public string Name { get; set; }

        public string HeroLocalizedName { get; set; }

        public SkillAbilityType AbilityBehavior { get; set; }

        public AffectsType Affects { get; set; }

        public DamageType DamageType { get; set; }

        public Boolean PiercesSpellImunity { get; set; }

        public Boolean Dispellable { get; set; }

        public string Description { get; set; }

        public List<string> Notes { get; set; }

        public string Image { get; set; }

        public string Link { get; set; }

        public List<KeyValuePair<string, string>> Effects { get; set; }

        public List<StatEffect> Stats { get; set; }

        public List<double> Cooldown { get; set; } 

        public List<int> ManaCost { get; set; }
    }
}
