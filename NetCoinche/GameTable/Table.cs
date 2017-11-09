using System;
using System.Linq;

namespace NetCoinche
{
    public class Table
    {
        private CardFamily atout;
        private Team[]     teams;
        private Card[]     midDeck;
        private Card[]     mainDeck;

        private Card       winningCard;
        private Player     winningCardPlayer;
        private int        sumCardDroppedPli;

        private GameState  state;
        private CardFamily firstCard;
        
        public bool checkMid()

        {
            if (this.midDeck[0] == null)
            {
                return true;
            }
            else {
                Console.WriteLine(this.midDeck[0].ToString());
                return false;
            }

        }

        public void initMidDeck()

        {
            for (int i = 0; i < this.midDeck.Length; i  += 1)
            {
                this.midDeck[i] = null;
            }

        }

        public string checkValueNonAtout(string first, string second)
        {
            if (first.equalsIgnoreCase("as") || first.equalsIgnoreCase("dix") && (!second.equalsIgnoreCase("as")) ||
                (first.equalsIgnoreCase("roi") && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as"))) ||
                (first.equalsIgnoreCase("dame") && (!second.equalsIgnoreCase("roi")) && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as"))) ||
                (first.equalsIgnoreCase("valet") && (!second.equalsIgnoreCase("dame")) && (!second.equalsIgnoreCase("roi")) && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as"))) ||
                (first.equalsIgnoreCase("dame") && (!second.equalsIgnoreCase("roi")) && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as")) && (!second.equalsIgnoreCase("neuf")) && (!second.equalsIgnoreCase("valet"))) ||
                ((first.equalsIgnoreCase("sept") || first.equalsIgnoreCase("huit") || first.equalsIgnoreCase("neuf")) && (!second.equalsIgnoreCase("valet")) && (!second.equalsIgnoreCase("dame")) && (!second.equalsIgnoreCase("roi")) && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as"))))
            {
                return first;
            }
            else
            {
                return second;
            }
        }
        
        public string checkValueAtout(string first, string second)
        {
            if (first.equalsIgnoreCase("valet") || first.equalsIgnoreCase("neuf") && (!second.equalsIgnoreCase("valet")) ||
                (first.equalsIgnoreCase("as") && (!second.equalsIgnoreCase("neuf")) && (!second.equalsIgnoreCase("valet"))) ||
                (first.equalsIgnoreCase("dix") && (!second.equalsIgnoreCase("as")) && (!second.equalsIgnoreCase("neuf")) && (!second.equalsIgnoreCase("valet"))) ||
                (first.equalsIgnoreCase("roi") && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as")) && (!second.equalsIgnoreCase("neuf")) && (!second.equalsIgnoreCase("valet"))) ||
                (first.equalsIgnoreCase("dame") && (!second.equalsIgnoreCase("roi")) && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as")) && (!second.equalsIgnoreCase("neuf")) && (!second.equalsIgnoreCase("valet"))) ||
                ((first.equalsIgnoreCase("sept") || first.equalsIgnoreCase("huit")) && (!second.equalsIgnoreCase("dame")) && (!second.equalsIgnoreCase("roi")) && (!second.equalsIgnoreCase("dix")) && (!second.equalsIgnoreCase("as")) && (!second.equalsIgnoreCase("neuf")) && (!second.equalsIgnoreCase("valet"))))
            {
                return first;
            }
            else
            {
                return second;
            }

        }
        
        public void PushCardOnMid(Card card, int index)
        {
            this.midDeck[index] = card;
        }
        
        public string getFormattedMidDeck()
        {
            string playerDeck = "[server.Table]";

            if (Tools.CountArray(this.midDeck) == 0)
                return "[server.Table] No server.Card\n";
            for (int i = 0; i < this.midDeck.Length; i += 1)
            {
                if (this.midDeck[i] != null)
                {
                    playerDeck += "[" + this.midDeck[i].FamilyCard.ToString() + "-" + this.midDeck[i].FamilyCard.ToString() + "]";
                    if (i < this.midDeck.Length)
                        playerDeck += " ";
                }
            }
            playerDeck += "\n";
            return playerDeck;
        }
        
        public Team getTeamOfPlayer(MyIp channel)
        {
            if (Server.mainTable.Teams[0].Players1[0].Channel == channel
                || Server.mainTable.Teams[0].Players1[1].Channel == channel)
                return Server.mainTable.Teams[0];
            else if (Server.mainTable.Teams[1].Players1[0].Channel == channel
                     || Server.mainTable.Teams[1].Players1[1].Channel == channel)
                return Server.mainTable.Teams[1];
            else
                return null;
        }
        
        public Card PickRandomCardInMainDeck()
        {
            int randomIndex = Tools.RandomInt(0, 32);
            while (this.mainDeck[randomIndex] == null)
                randomIndex = Tools.RandomInt(0, 32);
            Card pickedCard = this.mainDeck[randomIndex];
            this.mainDeck[randomIndex] = null;
            return pickedCard;
        }
        
        public void AddSumCardDropped(int value)
        {
            this.sumCardDroppedPli += value;
        }
        
        public void initMainDeck()
        {
            var     allFamily = Enum.GetValues(typeof(CardFamily)).Cast<CardFamily>().ToArray();
            var     allCard = Enum.GetValues(typeof(CardName)).Cast<CardName>().ToArray();
            
            var     mainDeckIndex = 0;

            for (var a = 0; a < this.mainDeck.Length; a += 1)
                this.mainDeck[a] = null;

            /* 4 family loop */
            for (var i = 0; i < 4; i += 1)
            {
                /* 8 card loop */
                for (var j = 0; j < 8; j += 1)
                {
                    this.mainDeck[mainDeckIndex] = new Card(allFamily[i], allCard[j]);
                    mainDeckIndex += 1;
                }
            }
        }
        
        public Table()
        {
            this.teams = new Team[2];
            this.teams[0] = new Team();
            this.teams[1] = new Team();
            this.midDeck = new Card[4];
            this.mainDeck = new Card[32];
            this.state = GameState.Init;
            this.sumCardDroppedPli = 0;
            this.initMainDeck();
        }

        public CardFamily Atout
        {
            get => atout;
            set => atout = value;
        }

        public Team[] Teams
        {
            get => teams;
            set => teams = value;
        }

        public Card[] MidDeck
        {
            get => midDeck;
            set => midDeck = value;
        }

        public Card[] MainDeck
        {
            get => mainDeck;
            set => mainDeck = value;
        }

        public Card WinningCard
        {
            get => winningCard;
            set => winningCard = value;
        }

        public Player WinningCardPlayer
        {
            get => winningCardPlayer;
            set => winningCardPlayer = value;
        }

        public int SumCardDroppedPli
        {
            get => sumCardDroppedPli;
            set => sumCardDroppedPli = value;
        }

        public GameState State
        {
            get => state;
            set => state = value;
        }

        public CardFamily FirstCard
        {
            get => firstCard;
            set => firstCard = value;
        }
    }
}