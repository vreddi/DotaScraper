using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataScraper
{
    [Serializable]
    public class Hero
    {

        public Hero() {

            this.Health = new HeroDistributedStats<float>();
            this.HealthRegen = new HeroDistributedStats<float>();
            this.Mana = new HeroDistributedStats<float>();
            this.ManaRegen = new HeroDistributedStats<float>();
            this.Armor = new HeroDistributedStats<float>();
            this.SpellDamage = new HeroDistributedStats<float>();
            this.AttacksPerSecond = new HeroDistributedStats<float>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LocalizedName { get; set; }

        public List<string> Roles { get; set; }

        public int Popularity { get; set; }

        public float WinRate { get; set; }

        public List<LanePresenceStats> LanePresence { get; set; }

        public string Primary { get; set; }

        public string Description { get; set; }

        public string Lore { get; set; }

        public string Image { get; set; }

        public string SourceLink { get; set; }

        public List<string> Aliases { get; set; }

        public List<Skill> Skills { get; set; }

        public HeroDistributedStats<float> Health { get; set; }

        public HeroDistributedStats<float> HealthRegen { get; set; }

        public HeroDistributedStats<float> Mana { get; set; }

        public HeroDistributedStats<float> ManaRegen { get; set; }

        public HeroDistributedStats<Range<float>> Damage { get; set; }

        public HeroDistributedStats<float> Armor { get; set; }

        public HeroDistributedStats<float> SpellDamage { get; set; }

        public HeroDistributedStats<float> AttacksPerSecond { get; set; }

        public int MovementSpeed { get; set; }

        public float TurnRate { get; set; }

        public HeroVisionRange VisionRange { get; set; }

        public int AttackRange { get; set; }

        public HeroAttackAnimation AttackAnimation { get; set; }

        public float BaseAttackTime { get; set; }

        public float MagicResitance { get; set; }

        public float CollisionSize { get; set; }

        public int Legs { get; set; }

        public DateTime LastUpdatedTimeStamp { get; set; }
    }
}
