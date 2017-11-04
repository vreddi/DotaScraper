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
        UnitTarget,
        NoTarget
    }

    public enum AffectsType {
        EnemyUnits,
        AlliedUnits,
        Creeps,
        Heroes
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

        public List<AffectsType> Affects { get; set; }

        public DamageType DamageType { get; set; }

        public Boolean PiercesSpellImmunity { get; set; }

        public Boolean Dispellable { get; set; }

        public string Description { get; set; }

        public string Lore { get; set; }

        public List<string> Notes { get; set; }

        public string Image { get; set; }

        public string Link { get; set; }

        public List<KeyValuePair<string, string>> Effects { get; set; }

        public List<StatEffect> Stats { get; set; }

        public List<double> Cooldown { get; set; } 

        public List<int> ManaCost { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientString"></param>
        /// <returns></returns>
        public static SkillAbilityType? GetSkillAbilityTypeFromString(string clientString)
        {
            string formattedString = clientString.Replace(" ", "").ToLower();

            switch (formattedString)
            {
                case "passive":
                    return SkillAbilityType.Passive;

                case "unittarget":
                    return SkillAbilityType.UnitTarget;

                case "pointtarget":
                    return SkillAbilityType.PointTarget;

                case "notarget":
                    return SkillAbilityType.NoTarget;

                default:
                    return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientString"></param>
        /// <returns></returns>
        public static List<AffectsType> GetAffectsTypeFromString(string clientString) {

            string formattedString = clientString.Replace(" ", "").ToLower();
            string[] affectsFormattedStrings = formattedString.Split(',');
            List<AffectsType> affectTypes = new List<AffectsType>();

            foreach (string affects in affectsFormattedStrings) {
                switch (affects)
                {
                    case "enemyunits":
                        affectTypes.Add(AffectsType.EnemyUnits);
                        break;

                    case "alliedunits":
                        affectTypes.Add(AffectsType.AlliedUnits);
                        break;

                    case "creeps":
                        affectTypes.Add(AffectsType.Creeps);
                        break;

                    case "heroes":
                        affectTypes.Add(AffectsType.Heroes);
                        break;

                    default:
                        break;
                }
            }

            return affectTypes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientString"></param>
        /// <returns></returns>
        public static bool? GetBooleanAnswerFromString(string clientString) {
            string formattedString = clientString.ToLower();

            switch (formattedString) {
                case "yes":
                    return true;

                case "no":
                    return false;

                case "cannot be dispelled":
                    return false;

                default:
                    return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientString"></param>
        /// <returns></returns>
        public static DamageType? GetDamageTypeFromString(string clientString)
        {

            string formattedString = clientString.Replace(" ", "").ToLower();

            switch (formattedString)
            {
                case "physical":
                    return DamageType.Physical;

                case "magical":
                    return DamageType.Magical;

                default:
                    return null;
            }
        }
    }
}
