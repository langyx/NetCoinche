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
            //Request server IP and port number
            Console.WriteLine("Please enter the server IP and port in the format 192.168.0.1:10000 and press return:");
            string serverInfo = Console.ReadLine();
 
            //Parse the necessary information out of the provided string
            string serverIP = "127.0.0.1";//serverInfo.Split(':').First();
            int serverPort = 4444;//int.Parse(serverInfo.Split(':').Last());
            int clientPort = -1;
            
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", PrintIncomingMessage);
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
            {
                if (clientPort == -1)
                    clientPort = localEndPoint.Port;
            }

            
            //Keep a loopcounter
            int loopCounter = 1;
            while (true)
            {
                //Write some information to the console window
                string messageToSend = "coucou" + ":" + clientPort.ToString();
             
                //Send the message in a single line
                NetworkComms.SendObject("Message", serverIP, serverPort, messageToSend);
 
                //Check if user wants to go around the loop
                Console.WriteLine("\nPress q to quit or any other key to send another message.");
                if (Console.ReadKey(true).Key == ConsoleKey.Q) break;
                else loopCounter++;
            }
 
            //We have used comms so we make sure to call shutdown
            NetworkComms.Shutdown();
        }
        
        private static void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("\nA message was received from " + connection.ToString() + " which said '" + message + "'.");
           
        }
    }
}