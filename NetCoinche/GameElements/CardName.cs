using System;

namespace NetCoinche
{
    public enum CardName
    {
        AS,
        Sept,
        Huit,
        Neuf,
        Dix,
        Valet,
        Dame,
        Roi,
        None
    }

    public static class CardNameExtension
    {

        public static int getValue(this CardName name, bool isAtout)
        {
            switch (name)
            {
                case CardName.AS:
                    return 11;
                case CardName.Sept: case CardName.Huit:
                    return 0;
                case CardName.Neuf:
                    return isAtout ? 14 : 0;
                case CardName.Dix:
                    return 10;
                case CardName.Valet:
                    return isAtout ? 20 : 2;;
                case CardName.Dame:
                    return 3;
                case CardName.Roi:
                    return 4;
                case CardName.None:
                    return 0;
                default:
                    return 0;
            }
        }
        
        public static string ToString(this CardName name)
        {
            switch (name)
            {
                case CardName.AS:
                    return "as";
                case CardName.Sept:
                    return "sept";
                case CardName.Huit:
                    return "huit";
                case CardName.Neuf:
                    return "neuf";
                case CardName.Dix:
                    return "dix";
                case CardName.Valet:
                    return "valet";
                case CardName.Dame:
                    return "dame";
                case CardName.Roi:
                    return "roi";
                case CardName.None:
                    return "none";
                default:
                    return "none";
            }
        }
    }
}