using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace MetadataScraper
{
    public static class SkillScraper
    {
        private static HttpClient Client = new HttpClient();

        // Dotabuff relevance
        private const string DotabuffHeroSkillEndpoint = "https://www.dotabuff.com/heroes/{0}/abilities";
        private const string DotabuffSkillColumnDiv = "col-8";
        private const string DotabuffSkillSectionBigAvatar = "bigavatar";
        private const string DotabuffSkillSectionEffects = "effects";
        private const string DotabuffSkillSectionDescription = "description";
        private const string DotabuffSkillSectionStats= "stats";
        private const string DotabuffSkillSectionCoolDownAndCost = "cooldown_and_cost";
        private const string DotabuffSkillSectionLore = "lore";

        private const string DotaBuffSkillSectionEffects_Behavior = "behavior";
        private const string DotaBuffSkillSectionEffects_Affects = "affects";
        private const string DotaBuffSkillSectionEffects_PiercesSpellImmunity = "spell_immunity_type";
        private const string DotaBuffSkillSectionEffects_Dispellable = "spell-dispellable-type";
        private const string DotaBuffSkillSectionEffects_DamageType = "damage_type";

        // Gamepedia relevance
        private const string GamepediaHeroInfoBoxClass = "infobox";
        private const string GamepediaHeroEvenRowsGray = "evenrowsgray";
        private const string GamepediaHeroOddRowsGray = "oddrowsgray";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="heroLocalizedName"></param>
        /// <returns></returns>
        public static List<Skill> ScrapeHeroSkills(string heroLocalizedName) {
            var htmlWeb = new HtmlWeb();
            string skillsDocument = string.Format(DotabuffHeroSkillEndpoint, heroLocalizedName);
            var dotaBuffHeroSkillsDoc = htmlWeb.Load(skillsDocument);

            ScraperConsole.LogInfo(string.Format("Loading document {0}", skillsDocument));

            HtmlNode SkillDivColumn = dotaBuffHeroSkillsDoc.DocumentNode.SelectSingleNode(
                    string.Format(".//div[@class = '{0}']", DotabuffSkillColumnDiv)
                );

            HtmlNodeCollection documentSections = SkillDivColumn.SelectNodes(".//section");

            List<Skill> heroSKills = new List<Skill>();

            foreach (HtmlNode section in documentSections) {
                HtmlNode header = section.SelectSingleNode(".//header");
                HtmlNode article = section.SelectSingleNode(".//article");
                HtmlNode bigAvatarDiv = article.SelectSingleNode(
                        string.Format(".//div[@class = '{0}']", DotabuffSkillSectionBigAvatar)
                    );
                HtmlNode effectsDiv = article.SelectSingleNode(
                        string.Format(".//div[@class = '{0}']", DotabuffSkillSectionEffects)
                    );
                HtmlNode descriptionDiv = article.SelectSingleNode(
                        string.Format(".//div[@class = '{0}']", DotabuffSkillSectionDescription)
                    );
                HtmlNode loreDiv = article.SelectSingleNode(
                        string.Format(".//div[@class = '{0}']", DotabuffSkillSectionLore)
                    );

                // header contains the name of the skill
                // clearing the element of the remaining junk
                try
                {
                    header.RemoveChild(header.SelectSingleNode(".//big"));
                }
                catch (Exception error) {
                }

                Skill skill = new Skill();

                // Parse Skill Name, Description, HeroLocalizedName and Image
                skill.Name = header.InnerText;
                skill.HeroLocalizedName = heroLocalizedName;
                skill.Image = "https:" + bigAvatarDiv.SelectSingleNode(".//img").GetAttributeValue("src", null);
                skill.Description = descriptionDiv.SelectSingleNode(".//p").InnerText;
                skill.Lore = loreDiv.InnerText;

                // Parse Skill's effect section
                try
                {
                    SkillScraper.ParseSkillEffects(effectsDiv, skill);
                }
                catch (Exception error)
                {
                    ScraperConsole.LogError("Error in Parsing Skill Effects");
                    ScraperConsole.LogError(error.ToString());
                }

                heroSKills.Add(skill);
            }

            return heroSKills;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectsDiv"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        private static Skill ParseSkillEffects(HtmlNode effectsDiv, Skill skill) {

            foreach (HtmlNode p in effectsDiv.SelectNodes(".//p")) {

                string className = p.SelectSingleNode(".//span").GetAttributeValue("class", null);

                switch (className)
                {
                    // AbilityBehavior
                    case DotaBuffSkillSectionEffects_Behavior:
                        p.RemoveChild(p.SelectSingleNode(string.Format(".//span[@class = '{0}']", DotaBuffSkillSectionEffects_Behavior)));
                        skill.AbilityBehavior = (SkillAbilityType)Skill.GetSkillAbilityTypeFromString(p.InnerText);
                        break;

                    // Affects
                    case DotaBuffSkillSectionEffects_Affects:
                        p.RemoveChild(p.SelectSingleNode(string.Format(".//span[@class = '{0}']", DotaBuffSkillSectionEffects_Affects)));
                        skill.Affects = Skill.GetAffectsTypeFromString(p.InnerText);
                        break;

                    // Pierces Spell Immunity
                    case DotaBuffSkillSectionEffects_PiercesSpellImmunity:
                        p.RemoveChild(p.SelectSingleNode(string.Format(".//span[@class = '{0}']", DotaBuffSkillSectionEffects_PiercesSpellImmunity)));
                        skill.PiercesSpellImmunity = (bool)Skill.GetBooleanAnswerFromString(p.InnerText);
                        break;


                    // Dispellable
                    case DotaBuffSkillSectionEffects_Dispellable:
                        p.RemoveChild(p.SelectSingleNode(string.Format(".//span[@class = '{0}']", DotaBuffSkillSectionEffects_Dispellable)));
                        skill.Dispellable = (bool)Skill.GetBooleanAnswerFromString(p.InnerText);
                        break;

                    // DamageType
                    case DotaBuffSkillSectionEffects_DamageType:
                        p.RemoveChild(p.SelectSingleNode(string.Format(".//span[@class = '{0}']", DotaBuffSkillSectionEffects_DamageType)));
                        skill.DamageType = (DamageType)Skill.GetDamageTypeFromString(p.InnerText);
                        break;

                    default:
                        break;
                }
            }

            return skill;
        }
    }
}