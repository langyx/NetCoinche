using System;
using System.Threading;

namespace NetCoinche
{
    public class Programm
    {
        public static int Preli(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Missing paramaters : NetCoinche.exe Port");
                System.Environment.Exit(0);
                return -1;
            }
            else
            {
                int argPort = int.TryParse(args[0], out argPort) ? argPort : -1;
                if (argPort == -1)
                {
                    Console.WriteLine("Bad paramaters : NetCoinche.exe Port");
                    System.Environment.Exit(0);
                } 
                return argPort;
            }
        }
        
        public static void Main(string[] args)
        {
            ServerHandler _serverHandler = new ServerHandler(Preli(args));
            Thread        _serverHandlerThread = new Thread(_serverHandler.run);
            
            Server        _server = new Server();
            Thread        _serverThread = new Thread(_server.Run);

            _serverHandlerThread.Start();
            _serverThread.Start();
        }
    }
}