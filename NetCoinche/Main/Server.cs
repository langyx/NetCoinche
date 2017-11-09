using System;
using System.Collections.Generic;
using NetworkCommsDotNet;

namespace NetCoinche
{
    public class Server
    {
        public static Table mainTable;
        public static List<Player> playerQueue;
        
        public static GameEngine gameEngigne;

        public Server()
        {
            Server.mainTable = new Table();
            Server.playerQueue = new List<Player>();
            Server.gameEngigne = new GameEngine();            
        }

        public void Run()
        {
            Server.gameEngigne.Run();
        }
        
        public static void writeMessageForAllPlayer(string message)
        {
          
            Team[] tableTeams = Server.mainTable.Teams;

            for (int i = 0; i < tableTeams[0].Players1.Length; i += 1)
            {
                if (tableTeams[0].Players1[i] != null)
                    Server.writeMessage(tableTeams[0].Players1[i].Channel, message);
            }

            for (int i = 0; i < tableTeams[1].Players1.Length; i += 1)
            {
                if (tableTeams[1].Players1[i] != null)
                    Server.writeMessage(tableTeams[1].Players1[i].Channel, message);
            }

            for (int i = 0; i < Server.playerQueue.Count; i += 1)
            {
                Player tempPlayer = Server.playerQueue[i];
                Server.writeMessage(tempPlayer.Channel, message);
            }

        }
        
        public static void writeMessage(MyIp client, string message)
        {
            NetworkComms.SendObject("Message", client.Ip, int.Parse(client.Port), message);
        }
        
        public static int getQueueIndex(MyIp channel)
        {
            for (int i = 0; i < Server.playerQueue.Count; i += 1)
            {
                Player temp = Server.playerQueue[i];
                if (temp.Channel == channel)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public static Player getPlayerByChannel(MyIp channel)
        {
            Team teamOne = Server.mainTable.Teams[0];
            Team teamTwo = Server.mainTable.Teams[1];


            for (int i = 0; i < teamOne.Players1.Length; i += 1)
            {
                if (teamOne.Players1[i] != null
                    && teamOne.Players1[i].Channel == channel)
                    return teamOne.Players1[i];
            }

            for (int i = 0; i < teamTwo.Players1.Length; i += 1)
            {
                if (teamTwo.Players1[i] != null
                    && teamTwo.Players1[i].Channel == channel)
                    return teamTwo.Players1[i];
            }

            for (int i = 0; i < Server.playerQueue.Count; i += 1)
            {
                Player waitingPlayer = Server.playerQueue[i];
                if (waitingPlayer.Channel == channel)
                    return waitingPlayer;
            }

            return null;
        }
        
        public static void showTablePlayers()
        {
            Team teamOne = Server.mainTable.Teams[0];
            Team teamTwo = Server.mainTable.Teams[1];

            Console.WriteLine("TeamOne :::");
            for (int i = 0; i < teamOne.Players1.Length; i += 1)
            {
                if (teamOne.Players1[i] != null)
                {
                    Console.WriteLine("server.Player : [" + teamOne.Players1[i].Name +
                                       "] on :" + teamOne.Players1[i].Channel.Ip);
                }
                else
                    Console.WriteLine("server.Player : No");
            }

            Console.WriteLine("TeamTwo :::");
            for (int i = 0; i < teamTwo.Players1.Length; i += 1)
            {
                if (teamTwo.Players1[i] != null)
                {
                    Console.WriteLine("server.Player : [" + teamTwo.Players1[i].Name +
                                       "] on :" + teamTwo.Players1[i].Channel.Ip);
                }
                else
                    Console.WriteLine("server.Player : No");
            }


            Console.WriteLine("\nserver.Player Queue :::");
            for (int i = 0; i < Server.playerQueue.Count; i += 1)
            {
                Player waitingPlayer = Server.playerQueue[i];
                Console.WriteLine("Waiter : " + waitingPlayer.Channel);
            }
        }
    }
}