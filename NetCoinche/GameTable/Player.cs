namespace NetCoinche
{
    public class Player
    {
        private Card[] deck;
        private string name;
        private MyIp channel;

        private static int count = 0;

        public Player(string name, MyIp channel)
        {
            this.name = name;
            this.channel = channel;
            this.deck = new Card[8];
        }
        
        public bool addCard(Card newCard) {
            if (newCard == null)
                return false;
            for (int i = 0; i < this.deck.Length; i += 1) {
                if (this.deck[i] == null) {
                    this.deck[i] = newCard;
                    return true;
                }
            }
            return false;
        }
        
        public string getFormatedDeck() {
            string playerDeck = "[Hand]";

            if (Tools.CountArray(this.deck) == 0)
                return "[Hand] No server.Card\n";
            for (int i = 0; i < this.deck.Length; i += 1) {
                if (this.deck[i] != null) {
                    playerDeck += "[" + this.deck[i].FamilyCard.ToString() + "-" + this.deck[i].FamilyName.ToString() + "]";
                    if (i < this.deck.Length)
                        playerDeck += " ";
                }
            }
            playerDeck += "\n";
            return playerDeck;
        }
        
        public bool searchFirstcardFamily(string str1) {
            int i = 0;
            while (i < this.deck.Length) {
                if (this.deck[i] == null) {
                    i += 1;
                } else if (this.deck[i] != null) {
                    if (str1.equalsIgnoreCase(this.deck[i].FamilyCard.ToString()))
                        return true;
                    i += 1;
                }
            }
            return false;
        }

        public bool searchAtout(string str1) {
            int i = 0;
            while (i < this.deck.Length) {
                if (this.deck[i] == null) {
                    i += 1;
                } else if (this.deck[i] != null) {
                    if (str1.equalsIgnoreCase(this.deck[i].FamilyCard.ToString()))
                        return true;
                    i += 1;
                }
            }
            return false;
        }
        
        public bool searchOnDeck(string str1, string str2) {
            int i = 0;
            while (i < this.deck.Length)
            {
                if (this.deck[i] == null) {
                    i += 1;
                } else if (this.deck[i] != null) {
                    if ((str1.equalsIgnoreCase(this.deck[i].FamilyName.ToString())) && (str2.equalsIgnoreCase(this.deck[i].FamilyCard.ToString()))) {
                        return true;
                    }
                    i += 1;
                }
            }
            return false;

        }


        public bool removeOnDeck(string str1, string str2)

        {
            Table table = new Table();
            GameEngine gameEngine = new GameEngine();
            if (Tools.CountArray(this.deck) == 0)
                return false;
            for (int i = 0; i < this.deck.Length; i += 1) {
                if (this.deck[i] != null) {
                    if ((str1.equalsIgnoreCase(this.deck[i].FamilyName.ToString()))
                        && (str2.equalsIgnoreCase(this.deck[i].FamilyCard.ToString())))
                    {
                        Server.mainTable.PushCardOnMid(this.deck[i], count);
                        if ((Server.mainTable.Atout.ToString().equalsIgnoreCase(this.deck[i].FamilyCard.ToString())))
                            Server.mainTable.AddSumCardDropped(this.deck[i].FamilyName.getValue(true));
                        else
                            Server.mainTable.AddSumCardDropped(this.deck[i].FamilyName.getValue(false));
                        this.deck[i] = null;
                        count += 1;
                        if (count == 4)
                            count = 0;
                    }

                }
            }
            return  true;
        }
        
        
        
        public Card[] Deck
        {
            get => deck;
            set => deck = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public MyIp Channel
        {
            get => channel;
            set => channel = value;
        }
    }
}