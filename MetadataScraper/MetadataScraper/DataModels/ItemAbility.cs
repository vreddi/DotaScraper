namespace MetadataScraper
{
    public enum ItemAbilityType {
        Active,
        Passive,
        Use
    }

    public class ItemAbility
    {
        public string Name { get; set; }
        
        public ItemAbilityType Type { get; set; }
        
        public string Description { get; set; }
        
        public int ManaCost { get; set; }
        
        public int CoolDown { get; set; }
    }
}
