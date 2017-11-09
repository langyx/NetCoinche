using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;

namespace ClientApplication
{
    class Program
    {
        
        public static  string serverIP = "127.0.0.1";
        public static  int    serverPort = 4444;
        public static  int    clientPort = -1;

        public static void ShowHelp()
        {
            Console.WriteLine("Missing Parameters : Client.exe Ip Port");
            System.Environment.Exit(0);
        }
        
        public static void GetAdress(string[] args)
        {
            if (args.Length < 2)
                Program.ShowHelp();
            Program.serverIP = args[0];
            int argPort = int.TryParse(args[1], out argPort) ? argPort : -1;
            if (argPort == -1)
                ShowHelp();
        }

        public static void RunObserver()
        {
            ConnectionObserver connectionObserver = new ConnectionObserver();
            Thread             connectionObserverThread = new Thread(connectionObserver.Run);
            connectionObserverThread.Start();
        }

        public static void MainLoop()
        {
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
                if (serverInfo.ToLower() == "exit")
                {
                    NetworkComms.Shutdown();
                    System.Environment.Exit(1);
                }
                if (serverInfo.Length > 0)
                {
                    var messageToSend = serverInfo + ":" + clientPort.ToString();
                    
                    NetworkComms.SendObject("Message", serverIP, serverPort, messageToSend);
                }
            }
 
        }
        
        static void Main(string[] args)
        {
            Program.GetAdress(args);
            
            Program.RunObserver();
            
            Program.MainLoop();
        }
        
        private static void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine(message);
        }
    }
}