using System;
using System.Collections.Generic;
using System.Linq;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;

namespace ClientApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverIP = "127.0.0.1";
            int serverPort = 4444;
            int clientPort = -1;
            
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", PrintIncomingMessage);
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
            {
                if (clientPort == -1)
                    clientPort = localEndPoint.Port;
            }

            
            while (true)
            {
                string serverInfo = Console.ReadLine();
                serverInfo = serverInfo.Replace('\n', ' ');
                if (serverInfo.Length > 0)
                {
                    string messageToSend = serverInfo + ":" + clientPort.ToString();
                    NetworkComms.SendObject("Message", serverIP, serverPort, messageToSend);
                }
            }
 
            NetworkComms.Shutdown();
        }
        
        private static void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine(message);
        }
    }
}