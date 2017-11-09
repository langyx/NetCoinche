using System;

namespace NetCoinche
{
    public enum CardFamily
    {
        Pique,
        Coeur,
        Trefle,
        Carreau,
        None
    }
    
    public static class CardFamilyExtension
    {        
        public static CardFamily GetFamily(this string name)
        {
            switch (name.ToLower())
            {
                case "pique":
                    return CardFamily.Pique;

                case "coeur":
                    return CardFamily.Coeur;

                case "trefle":
                    return CardFamily.Trefle;

                case "carreau":
                    return CardFamily.Carreau;
                    
                default:
                    return CardFamily.None;

            }
        }
        
        
        public static string ToString(this CardFamily family)
        {
            switch (family)
            {
                case CardFamily.Carreau:
                    return "carreau";
                
                case CardFamily.Coeur:
                    return "coeur";

                case CardFamily.Trefle:
                    return "trefle";
                
                case CardFamily.Pique:
                    return "pique";

                case CardFamily.None:
                    return "none";
                    
                default:
                    return "none";

            }
        }
    }

}