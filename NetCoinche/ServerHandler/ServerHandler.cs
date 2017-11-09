using System;
using System.Collections.Generic;
using System.Threading;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using static System.Int32;

/*foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
               Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);
*/

namespace NetCoinche
{
    class ServerHandler
    {
        private bool                    _running;
        private int                     _port;  
        
        public bool Running
        {
            get => _running;
            set => _running = value;
        }

        public ServerHandler(int newPort)
        {
            _running = true;
            _port = newPort;
        }

        public void run()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", ReceiveMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Connection", ConnectionClientObserver);
            NetworkComms.AppendGlobalConnectionEstablishHandler(ClientConnected);
            NetworkComms.AppendGlobalConnectionCloseHandler(ClientDisconnected);
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, _port));
            while (_running){}
            NetworkComms.Shutdown();
        }

        private void ConnectionClientObserver(PacketHeader header, Connection connection, string msg)
        {
            
        }
        
        public void Stop()
        {
            _running = false;
        }
 
        private void ReceiveMessage(PacketHeader header, Connection connection, string message)
        {
           var adressClient = Tools.getIpPortFromString(connection.ConnectionInfo.RemoteEndPoint.ToString());
           this.ManageCommand(adressClient, Tools.getMessageFromClient(message));
        }
        
        private void ClientConnected(Connection connection)
        {
            Console.WriteLine("A new client connected - " + connection.ConnectionInfo.RemoteEndPoint.ToString());
            this.manageNewPlayer(Tools.getIpPortFromString(connection.ConnectionInfo.RemoteEndPoint.ToString()), connection);
        }

        private void manageNewPlayer(MyIp ctx, Connection connection)
        {
            Player newPlayer = new Player(Tools.randomPlayerName(), ctx);

            int countPlayerTeamOne = Tools.CountArray(Server.mainTable.Teams[0].Players1);
            int countPlayerTeamTwo = Tools.CountArray(Server.mainTable.Teams[1].Players1);

            if (countPlayerTeamOne < 2)
                Server.mainTable.Teams[0].SetNewPlayer(newPlayer, Server.mainTable.Teams[0].getFirstFreePlayerIndex());
            else if (countPlayerTeamTwo < 2)
                Server.mainTable.Teams[1].SetNewPlayer(newPlayer, Server.mainTable.Teams[1].getFirstFreePlayerIndex());
            else if (Server.playerQueue.Count < 2)
                Server.playerQueue.Add(newPlayer);
            else
            {
                Server.writeMessage(ctx, "Server & Queue Full");
                connection.CloseConnection(false, 0);
                return;
            }
            Server.writeMessage(ctx, "[Server] Welcome " + newPlayer.Name + "\n");
        }

        private void ClientDisconnected(Connection connection)
        {
            MyIp clientIp = Tools.getIpPortFromString(connection.ConnectionInfo.RemoteEndPoint.ToString());
            if (Server.mainTable.Teams[0].RemovePlayer(clientIp)
                || Server.mainTable.Teams[1].RemovePlayer(clientIp)) {
                if (Server.playerQueue.Count > 0) {
                    Player subPlayerFromQueue = Server.playerQueue[0];
                    manageNewPlayer(subPlayerFromQueue.Channel, connection);
                    Server.playerQueue.Remove(subPlayerFromQueue);
                }
                Console.WriteLine("Client : " + clientIp.Ip + ":" + clientIp.Port + " has left !");
            } else {
                int qeueIndex = Server.getQueueIndex(clientIp);
                Player subPlayerFromQueue = Server.playerQueue[qeueIndex];
                if (qeueIndex != -1)
                    Server.playerQueue.Remove(subPlayerFromQueue);
            }
            Server.showTablePlayers();
        }
        
        private void ManageCommand(MyIp channel, string message)
        {
           Console.WriteLine(message);
           Server.showTablePlayers();
            
            String[] command = message.Split(' ');
            Player currPlayer = Server.getPlayerByChannel(channel);
            Team playerTeam = Server.mainTable.getTeamOfPlayer(currPlayer.Channel);


            switch (command[0].ToLower())
            {
                case "name":
                    if (command.Length == 2)
                        Server.getPlayerByChannel(channel).Name = command[1];
                    else
                        Server.writeMessage(channel, "[Server] Command Bad Arguments\n");
                    break;

                case "hand":
                    Server.writeMessage(channel, currPlayer.getFormatedDeck());
                    break;

                case "table":
                    Server.writeMessage(channel, Server.mainTable.getFormattedMidDeck());
                    break;

                case "bet":
                    if (Server.mainTable.State == GameState.Bet && Server.gameEngigne.isTheGamePlayable())
                    {
                        if (Server.gameEngigne.isThePlayerCanPlay(channel))
                        {
                            int currentTeamOneBet;
                            int currentTeamTwoBet;
                            int currentBestBet;
                            switch (command.Length)
                            {
                                case 2: /* case of simple action */
                                    switch (command[1].ToLower())
                                    {
                                        case "coinche":
                                            if (playerTeam.IsCoinched())
                                            {
                                                Server.writeMessage(channel,
                                                    "[Bet] You're coinched : Passe Or Surcoinche\n");
                                                break;
                                            }
                                            currentTeamOneBet = Server.mainTable.Teams[0].Bet;
                                            currentTeamTwoBet = Server.mainTable.Teams[1].Bet;
                                            currentBestBet = Math.Max(currentTeamOneBet, currentTeamTwoBet);
                                            Team bestBetTeam = null;

                                            /* On vérifie qu'une des team à bet */
                                            if (currentBestBet == 0)
                                            {
                                                Server.writeMessage(channel, "[Bet] No bet to coinche\n");
                                                break;
                                            }

                                            if (currentBestBet == currentTeamOneBet)
                                                bestBetTeam = Server.mainTable.Teams[0];
                                            else
                                                bestBetTeam = Server.mainTable.Teams[1];

                                            /* On vérifie que le meilleur paris ne soit pas l'équipe du joueur voulant coincher */
                                            MyIp playerAdress = currPlayer.Channel;
                                            if (playerAdress == bestBetTeam.Players1[0].Channel
                                                || playerAdress == bestBetTeam.Players1[1].Channel)
                                            {
                                                Server.writeMessage(channel,
                                                    "[Bet] No bet to coinche (You team are highest Bet)\n");
                                                break;
                                            }

                                            bestBetTeam.Coinche = 2;
                                            Server.gameEngigne.GoBackPlayerTurn();

                                            Player lastPlayer =
                                                Server.gameEngigne.getPlayerByMapPosition(Server.gameEngigne.PlayerTurn);
                                            Server.writeMessageForAllPlayer(
                                                "[Bet] [" + currPlayer.Name + "] Coinche\n");
                                            Server.writeMessageForAllPlayer(
                                                "[Bet] " + lastPlayer.Name +
                                                " have to answer (Surcoinche or Passe) !\n");
                                            break;

                                        case "surcoinche":
                                            if (playerTeam.Coinche <= 0)
                                            {
                                                Server.writeMessage(channel,
                                                    "[Bet] You can't surcoinche because nobody coinche you\n");
                                                break;
                                            }

                                            playerTeam.Coinche = 3;

                                            Server.gameEngigne.TableCycle = 0;
                                            Server.gameEngigne.PlayerTurn = 0;
                                            Server.mainTable.State = GameState.BetTraitement;

                                            break;

                                        case "passe":

                                            Server.writeMessageForAllPlayer(
                                                "[Bet] [" + currPlayer.Name + "] Passe\n");
                                            Server.gameEngigne.GoNextPlayerTurn();

                                            /* Redistribution des cartes dans le cas ou tous les joueurs ont passé pendant le tour de bet sans aucune bet placée */
                                            if (Server.gameEngigne.PlayerTurn == 0 &&
                                                Server.gameEngigne.getBestBet() <= 0)
                                            {
                                                Server.mainTable.initMainDeck();
                                                if (Server.gameEngigne.DistributeCardsForPlayers())
                                                    Server.gameEngigne.TableCycle = 0;

                                            }
                                            /* Passage au jeu (pli) dans le cas ou le dernier passe alors qu'une bet plus élevé que celui de sa team est placée */
                                            else if (Server.gameEngigne.PlayerTurn == 0 &&
                                                     Server.gameEngigne.getBestBet() > playerTeam.Bet)
                                            {
                                                Server.mainTable.State = GameState.BetTraitement;
                                            }
                                            /* Si le joueur Passe alors que sa Team est coinché (il ne surcoinche pas) */
                                            else if (playerTeam.IsCoinched())
                                            {
                                                Server.mainTable.State = GameState.BetTraitement;
                                            }
                                            break;

                                        default:
                                            Server.writeMessage(channel, "[Bet] Bad argument\n");
                                            break;
                                    }
                                    break;
                                case 3: /* case of betting on color Form : <family> <amount> */

                                    /* Si le player est coinché il ne peut pas remiser */
                                    if (playerTeam.IsCoinched())
                                    {
                                        Server.writeMessage(channel, "[Bet] You're coinched : Passe Or Surcoinche\n");
                                        break;
                                    }

                                    /* check de la famille de la carte choisie */
                                    CardFamily cardFamily = command[1].GetFamily();
                                    if (cardFamily == CardFamily.None)
                                    {
                                        Server.writeMessage(channel, "[Bet] Bad family arg\n");
                                        break;
                                    }

                                    /* Check du format du montant de paris */
                                    int amount = Tools.tryParse(command[2]);
                                    if (amount == -1 || amount % 10 != 0 || amount < 80 || amount > 160)
                                    {
                                        Server.writeMessage(channel, "[Bet] Bad amount arg\n");
                                        break;
                                    }

                                    /* check que le bet proposé est plus grand que celui des deux team */
                                    currentTeamOneBet = Server.mainTable.Teams[0].Bet;
                                    currentTeamTwoBet = Server.mainTable.Teams[1].Bet;
                                    currentBestBet = Math.Max(currentTeamOneBet, currentTeamTwoBet);
                                    if (amount <= currentBestBet)
                                    {
                                        Server.writeMessage(channel,
                                            "[Bet] amount have to be biggest than current best Bet (" + currentBestBet +
                                            ")\n");
                                        break;
                                    }

                                    /* Push du nouveau hight bet */
                                    Server.mainTable.getTeamOfPlayer(channel).Bet =  amount;
                                    Server.mainTable.getTeamOfPlayer(channel).BetFamily = cardFamily;

                                    Server.writeMessageForAllPlayer(
                                        "[Bet] [" + currPlayer.Name + "] New hight bet : " + amount + "\n");

                                    /* passage au joueur suivant */
                                    Server.gameEngigne.GoNextPlayerTurn();

                                    break;

                                default:
                                    Server.writeMessage(channel, "[Bet] Bad argument\n");
                                    break;
                            }
                        }
                        else
                            Server.writeMessage(channel, "[Bet] Is not you turn\n");
                    }
                    else
                        Server.writeMessage(channel, "[Server] Is no the bet round !\n");
                    break;

                case "drop":
                    if (Server.gameEngigne.isTheGamePlayable() && Server.mainTable.State == GameState.Pli)
                    {
                        if (Server.gameEngigne.isThePlayerCanPlay(channel))
                        {
                            if (!((command.Length == 3) && ((command[1].equalsIgnoreCase(CardName.AS.ToString())) ||
                                                            command[1].equalsIgnoreCase(CardName.Sept.ToString()) ||
                                                            command[1].equalsIgnoreCase(CardName.Huit.ToString()) ||
                                                            command[1].equalsIgnoreCase(CardName.Neuf.ToString()) ||
                                                            command[1].equalsIgnoreCase(CardName.Dix.ToString()) ||
                                                            command[1].equalsIgnoreCase(CardName.Valet.ToString()) ||
                                                            command[1].equalsIgnoreCase(CardName.Dame.ToString()) ||
                                                            command[1].equalsIgnoreCase(CardName.Roi.ToString())) &&
                                  (command[2].equalsIgnoreCase(CardFamily.Coeur.ToString()) ||
                                   command[2].equalsIgnoreCase(CardFamily.Pique.ToString()) ||
                                   command[2].equalsIgnoreCase(CardFamily.Trefle.ToString()) ||
                                   command[2].equalsIgnoreCase(CardFamily.Carreau.ToString()))))
                            {
                                Server.writeMessage(channel, "[Server] Please enter drop Cardname CardFamily\n");
                            }
                            else
                            {
                                if (Server.mainTable.MidDeck[0] == null)
                                {
                                    if (Server.getPlayerByChannel(channel).searchOnDeck(command[1], command[2]))
                                    {
                                        Server.getPlayerByChannel(channel).removeOnDeck(command[1], command[2]);
                                        Server.writeMessage(channel, "[Server] Card Dropped\n");
                                        Server.mainTable.FirstCard = command[2].GetFamily();
                                        Server.mainTable.WinningCard = Server.mainTable.MidDeck[0];
                                        Server.mainTable.WinningCardPlayer = currPlayer;
                                        Server.gameEngigne.GoNextPlayerTurn();
                                        // PLAYER OBLIGE DE METTRE LE MEME TYPE QUE LA CARTE DU MILIEU
                                    }
                                    else
                                    {
                                       Server.writeMessage(channel, "You don't have this card or already used, try again !");
                                    }
                                }
                                else
                                {
                                    // si il a trouvé la carte dans le deck
                                    if (Server.getPlayerByChannel(channel).searchOnDeck(command[1], command[2]))
                                    {
                                        // Check si la carte que on veut poser est égale à la famille de la premiere
                                        if (Server.mainTable.FirstCard.ToString()
                                            .equalsIgnoreCase(command[2]))
                                        {
                                            //Check si la first card et celle qu'on veut poser sont de la famille de l'atout
                                            if (Server.mainTable.FirstCard.ToString()
                                                .equalsIgnoreCase(Server.mainTable.Atout.ToString()))
                                            {
                                                //si famille de Atout condition de check de valeur
                                                Server.getPlayerByChannel(channel).removeOnDeck(command[1], command[2]);
                                                Server.writeMessage(channel, "[Server] Card Dropped\n");
                                                if (Server.gameEngigne.PlayerTurn < 4)
                                                {
                                                    //Server.mainTable.setWinningCard();
                                                    if (Server.mainTable.checkValueAtout(
                                                            Server.mainTable.WinningCard.FamilyName
                                                                .ToString(),
                                                            Server.mainTable.MidDeck[Server.gameEngigne.PlayerTurn].FamilyName
                                                                .ToString()) ==
                                                        Server.mainTable.MidDeck[
                                                                Server.gameEngigne.PlayerTurn].FamilyName
                                                            .ToString())
                                                    {
                                                        Server.mainTable.WinningCard = Server.mainTable.MidDeck[Server.gameEngigne.PlayerTurn];
                                                        Server.mainTable.WinningCardPlayer = currPlayer;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Server.getPlayerByChannel(channel).removeOnDeck(command[1], command[2]);
                                                Server.writeMessage(channel, "[Server] Card Dropped\n");
                                                if (Server.gameEngigne.PlayerTurn < 4)
                                                {
                                                    if (Server.mainTable.checkValueNonAtout(
                                                            Server.mainTable.WinningCard.FamilyName
                                                                .ToString(),
                                                            Server.mainTable.MidDeck[
                                                                    Server.gameEngigne.PlayerTurn].FamilyName
                                                                .ToString()) ==
                                                        Server.mainTable.MidDeck[
                                                                Server.gameEngigne.PlayerTurn].FamilyName
                                                            .ToString())
                                                    {
                                                        Server.mainTable.WinningCardPlayer = currPlayer;
                                                        Server.mainTable.WinningCard = Server.mainTable.MidDeck[Server.gameEngigne.PlayerTurn];
                                                    }
                                                }
                                            }
                                            Server.gameEngigne.GoNextPlayerTurn();
                                        }
                                        else // si la carte qu'on veut poser est pas égale a la famille de la premiere donc peut etre pas égale a l'atout
                                            // SI egal a atout et egal a family ca va pas rentrer dedans
                                            //si egal a atout mais pas égal a family , rentre ici !
                                        {
                                            // Si il a trouvé une carte de la famille de la 1ere card
                                            if (Server.getPlayerByChannel(channel)
                                                .searchFirstcardFamily(Server.mainTable.FirstCard
                                                    .ToString()))
                                            {
                                                Server.writeMessage(channel,
                                                    "You have a " +
                                                    Server.mainTable.WinningCard.FamilyCard.ToString() +
                                                    " in your deck, play it\n");
                                            }
                                            else
                                            {
                                                //check si il pisse de l'atout ou pas ( SI C PAS DE LA MEME FAMILLE MAIS ATOUT )
                                                if (command[2].equalsIgnoreCase(Server.mainTable.Atout
                                                        .ToString()) &&
                                                    (Server.getPlayerByChannel(channel)
                                                         .searchFirstcardFamily(Server.mainTable.FirstCard
                                                             .ToString()) == false))
                                                {
                                                    // if winning card est atout check value des deux
                                                    // if winning card != atout
                                                    //wining card == command[2]
                                                    if (Server.mainTable.WinningCard.FamilyCard.ToString()
                                                        .equalsIgnoreCase(Server.mainTable.Atout.ToString()))
                                                    {
                                                        Server.getPlayerByChannel(channel)
                                                            .removeOnDeck(command[1], command[2]);
                                                        Server.writeMessage(channel, "[Server] Card Dropped\n");

                                                        if (Server.gameEngigne.PlayerTurn < 4)
                                                        {
                                                            if (Server.mainTable.checkValueAtout(
                                                                    Server.mainTable.WinningCard.FamilyName
                                                                        .ToString(),
                                                                    Server.mainTable.MidDeck[
                                                                            Server.gameEngigne.PlayerTurn]
                                                                        .FamilyName.ToString()) ==
                                                                Server.mainTable.MidDeck[
                                                                        Server.gameEngigne.PlayerTurn]
                                                                    .FamilyName
                                                                    .ToString())
                                                            {
                                                                Server.mainTable.WinningCard = Server.mainTable.MidDeck[Server.gameEngigne.PlayerTurn];
                                                                Server.mainTable.WinningCardPlayer = currPlayer;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // LE CAS OU JE COUPE AVEC DE LATOUT
                                                        Server.getPlayerByChannel(channel)
                                                            .removeOnDeck(command[1], command[2]);
                                                        Server.writeMessage(channel, "[Server] Card Dropped\n");
                                                        if (Server.mainTable.WinningCard.FamilyCard.ToString().Equals(Server.mainTable.Atout.ToString()))
                                                        {
                                                            if (Server.mainTable.checkValueAtout(
                                                                    Server.mainTable.WinningCard.ToString(),
                                                                    command[2]) == command[2])
                                                            {
                                                                Server.mainTable.WinningCard = Server.mainTable.MidDeck[Server.gameEngigne.PlayerTurn];
                                                                Server.mainTable.WinningCardPlayer = currPlayer;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Server.mainTable.WinningCard = Server.mainTable.MidDeck[Server.gameEngigne.PlayerTurn];
                                                            Server.mainTable.WinningCardPlayer = currPlayer;
                                                        }
                                                    }
                                                }
                                                else // LE CAS OU JE JETTE JUSTE DE LA MERDE SANS ATOUT SANS RIEN
                                                {
                                                    Server.getPlayerByChannel(channel)
                                                        .removeOnDeck(command[1], command[2]);
                                                    Server.writeMessage(channel, "[Server] Card Dropped\n");
                                                }
                                                Server.gameEngigne.GoNextPlayerTurn();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Server.writeMessage(channel,
                                            "[Server] You don't have this card or already used, try again !\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Server.writeMessage(channel, "[Server] You can't drop ! Not your turn\n");
                        }
                    }
                    break;


                default:
                    Server.writeMessage(channel, "[Server] Command Unkown\n");
                    break;
            }


            /*  NetworkComms.SendObject("Message", adressClient[0], Tools.getPortFromString(message), "nique ta mere");*/
        }
        
        
    }
}