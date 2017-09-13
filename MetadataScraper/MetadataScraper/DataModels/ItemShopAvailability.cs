using System;

namespace MetadataScraper
{
    public class ItemShopAvailability
    {
        public Boolean SideShop { get; set; }

        public Boolean HomeShop { get; set; }

        public Boolean SecretShop { get; set; }

        public ItemShopAvailability DetermineShopBasedOnText(string text) {

            string lowerCaseText = text.ToLower();

            if (lowerCaseText.Contains("side")) {
                this.SideShop = true;
            }

            if (lowerCaseText.Contains("home") || lowerCaseText.Contains("base") || lowerCaseText.Contains("main")) {
                this.HomeShop = true;
            }

            if (lowerCaseText.Contains("secret")) {
                this.SecretShop = true;
            }

            return this;
        }
    }
}
