namespace NetCoinche
{
    public class Team
    {
        private Player[]       Players;
        private int            score;
        private int            bet;
        private CardFamily     betFamily;
        private int            coinche; // 1 disable // 2 coinched // 3 surcoinched
        private int            gameScore;

        public Team()
        {
            this.score = 0;
            this.bet = 0;
            this.coinche = 0;
            this.gameScore = 0;
            this.Players = new Player[2];
        }
        
        public void SetGameScore(int gameScore)
        {
            this.gameScore += gameScore;
        }
        
        public void AddScore(int score)
        {
            this.score += score;
        }
        
        public int getFirstFreePlayerIndex()
        {
            for (var i = 0; i < this.Players.Length; i += 1)
            {
                if (this.Players[i]== null)
                    return i;
            }
            return -1;
        }
        
        public bool RemovePlayer(MyIp channelPlayer)
        {
            for (var i = 0; i < this.Players.Length; i += 1)
            {
                if (this.Players[i] != null
                    && this.Players[i].Channel == channelPlayer)
                {
                    this.Players[i] = null;
                    return true;
                }
            }
            return false;
        }
        
        public void SetNewPlayer(Player newPlayer, int indexPlayer)
        {
            this.Players[indexPlayer] = newPlayer;
        }

        public bool IsCoinched()
        {
            return (this.coinche > 0);
        }

        public Player[] Players1
        {
            get => Players;
            set => Players = value;
        }

        public int Score
        {
            get => score;
            set => score = value;
        }

        public int Bet
        {
            get => bet;
            set => bet = value;
        }

        public CardFamily BetFamily
        {
            get => betFamily;
            set => betFamily = value;
        }

        public int Coinche
        {
            get => coinche;
            set => coinche = value;
        }

        public int GameScore
        {
            get => gameScore;
            set => gameScore = value;
        }
    }
    
    
}