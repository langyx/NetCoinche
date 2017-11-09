using System;
using System.Threading;

namespace NetCoinche
{
    public class GameEngine
    {
        protected int playerTurn = 0;
        protected int tableCycle = 0;

        public GameEngine()
        {
        }

        public void Run()
        {
            while (true)
            {
                GamingCore();
                Thread.Sleep(100);
            }
        }

        protected void GamingCore()
        {
            while (!this.isTheGamePlayable())
            {
                //Server.writeMessageForAllPlayer("[Server] Waiting for full team\n");
                Thread.Sleep(1000);
            }

            switch (Server.mainTable.State)
            {
                
                case GameState.Init: //Distribution des cards aux players

                    bool distribState = this.DistributeCardsForPlayers();

                    if (distribState)
                        Server.mainTable.State = GameState.Bet;
                    break;

                case GameState.Bet:
                    /* All manage in ServerHandler */

                    /* Si les paris s'éternise sur plus de 3 tours */
                    if (this.TableCycle == 3)
                        Server.mainTable.State = GameState.BetTraitement;
                    break;

                case GameState.BetTraitement:
                    Team attackingTeam = this.getBestBetTeam(false);
                    Team defenseTeam = this.getBestBetTeam(true);

                    defenseTeam.Bet = 0;
                    defenseTeam.Coinche = 0;

                    Server.mainTable.Atout = attackingTeam.BetFamily;

                    Server.writeMessageForAllPlayer("[Server] Start Game ! Atout is [" +
                                                    attackingTeam.BetFamily.ToString() + "]\n");

                    Server.mainTable.State = GameState.Pli;
                    break;

                case GameState.Waitting:
                    break;

                case GameState.Pli:
                    if (Server.gameEngigne.PlayerTurn == 3)
                    {
                        while (Server.gameEngigne.PlayerTurn != 0)
                        {
                            Thread.Sleep(100);
                        }

                        int sumRoundPoint = Server.mainTable.SumCardDroppedPli;
                        Server.writeMessageForAllPlayer("[Server] Round ended by " + sumRoundPoint + " points \n");
                        Server.mainTable.getTeamOfPlayer(Server.mainTable.WinningCardPlayer.Channel).AddScore(sumRoundPoint);
                        Server.mainTable.SumCardDroppedPli = 0;
                        Server.mainTable.initMidDeck();
                        Server.mainTable.WinningCard = null;
                        Server.mainTable.WinningCardPlayer = null;

                        if (Tools.CountArray(Server.mainTable.Teams[0].Players1[0].Deck) == 0)
                        {
                            Team attTeam = this.getBestBetTeam(false);
                            Team defTeam = this.getBestBetTeam(true);
                            Server.mainTable.State = GameState.Ended;

                            int scoreAtt = attTeam.Score;
                            int scoreDef = defTeam.Score;

                            int finalAttScore = 0;
                            int finalDefScore = 0;

                            int coincheMultiplicator = attTeam.Coinche > 0 ? attTeam.Coinche : 1;

                            if (scoreAtt >= attTeam.Bet)
                            {
                                Server.writeMessageForAllPlayer("Attacking team succeeded!\n");
                                finalAttScore = scoreAtt + (attTeam.Bet * coincheMultiplicator);
                                finalDefScore = scoreDef;
                            }
                            else
                            {
                                Server.writeMessageForAllPlayer("Attacking team failed!\n");
                                finalDefScore = scoreDef + (attTeam.Bet * coincheMultiplicator);
                            }

                            String messageRoundScore = "Round Score " + finalAttScore.ToString() + " vs "
                                                       + finalDefScore.ToString() + "\n";
                            Server.writeMessageForAllPlayer(messageRoundScore);


                            this.getBestBetTeam(false).GameScore = finalAttScore;
                            this.getBestBetTeam(true).GameScore = finalDefScore;

                            String messageGameScore = "Game Score "
                                                      + this.getBestBetTeam(false).GameScore.ToString() +
                                                      " vs "
                                                      + this.getBestBetTeam(true).GameScore.ToString() +
                                                      "\n";
                            Server.writeMessageForAllPlayer(messageGameScore);

                            Server.mainTable.State = GameState.Ended;
                        }
                    }
                    break;

                case GameState.Ended:

                    Server.writeMessageForAllPlayer("New round in 3 seconds....");

                    Thread.Sleep(3000);
                   
                    Server.mainTable.initMainDeck();
                    Server.mainTable.initMidDeck();
                    this.DistributeCardsForPlayers();

                    Server.mainTable.State = GameState.Bet;

                    break;

                default:
                    break;
            }

            //System.out.println("Game is runnig");

        }

        public int getPlayerMapPostion(MyIp playerChan)
        {
            if (Server.mainTable.Teams[0].Players1[0].Channel == playerChan)
                return 0;
            else if (Server.mainTable.Teams[0].Players1[1].Channel == playerChan)
                return 2;
            else if (Server.mainTable.Teams[1].Players1[0].Channel == playerChan)
                return 1;
            else if (Server.mainTable.Teams[1].Players1[1].Channel == playerChan)
                return 3;
            else
                return -1;
        }

        public Player getPlayerByMapPosition(int mapPos)
        {
            switch (mapPos)
            {
                case 0:
                    return Server.mainTable.Teams[0].Players1[0];
                case 1:
                    return Server.mainTable.Teams[1].Players1[0];
                case 2:
                    return Server.mainTable.Teams[0].Players1[1];
                case 3:
                    return Server.mainTable.Teams[1].Players1[1];
                default:
                    return null;

            }
        }
        
        public bool isThePlayerCanPlay(MyIp player)
        {
            if (this.getPlayerMapPostion(player) == this.playerTurn)
                return true;
            else
                return false;
        }

        public bool isTheGamePlayable()
        {
            Team[] tableTeams = Server.mainTable.Teams;

            if (Tools.CountArray(tableTeams[0].Players1) == 2
                && Tools.CountArray(tableTeams[1].Players1) == 2)
                return true;
            else
                return false;
        }
        
        public void GoNextPlayerTurn()
        {
            Thread.Sleep(100);
            
            if (this.playerTurn == 3)
            {
                this.playerTurn = 0;
                this.tableCycle += 1;
            }
            else
                this.playerTurn += 1;
        }

        public void GoBackPlayerTurn()
        {
            if (this.playerTurn == 0)
                this.playerTurn = 3;
            else
                this.playerTurn -= 1;
        }
        
        public bool DistributeCardsForPlayers()
        {
            bool distribState = true;

            if (!initDeckPlayerWithMainDeck(Server.mainTable.Teams[0].Players1[0])) distribState = false;
            if (!initDeckPlayerWithMainDeck(Server.mainTable.Teams[0].Players1[1])) distribState = false;
            if (!initDeckPlayerWithMainDeck(Server.mainTable.Teams[1].Players1[0])) distribState = false;
            if (!initDeckPlayerWithMainDeck(Server.mainTable.Teams[1].Players1[1])) distribState = false;

            String distribStateMessage = "a";
            if (distribState == false)
                distribStateMessage = "[Server] Cards distribution failed !\n";
            else
                distribStateMessage = "[Server] Cards distribution done !\n";

            Console.WriteLine(distribStateMessage);
            Server.writeMessageForAllPlayer(distribStateMessage);
            return distribState;
        }
        
        public int getBestBet()
        {
            int currentTeamOneBet = Server.mainTable.Teams[0].Bet;
            int currentTeamTwoBet = Server.mainTable.Teams[1].Bet;
            int currentBestBet = Math.Max(currentTeamOneBet, currentTeamTwoBet);
            return currentBestBet;
        }

        public Team getBestBetTeam(bool inversed)
        {
            if (this.getBestBet() == Server.mainTable.Teams[0].Bet)
                return inversed ? Server.mainTable.Teams[1] :  Server.mainTable.Teams[0];
            else
                return inversed ? Server.mainTable.Teams[0] :  Server.mainTable.Teams[1];

        }
        
        private bool initDeckPlayerWithMainDeck(Player player)
        {
            for (int i = 0; i < player.Deck.Length; i +=  1)
            {
                player.Deck[i] = null;
                if (player.addCard(Server.mainTable.PickRandomCardInMainDeck()) == false)
                    return false;
            }
            return  true;
        }

        public int PlayerTurn
        {
            get => playerTurn;
            set => playerTurn = value;
        }

        public int TableCycle
        {
            get => tableCycle;
            set => tableCycle = value;
        }
    }
}