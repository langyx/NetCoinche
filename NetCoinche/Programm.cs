using System;
using System.Threading;

namespace NetCoinche
{
    public class Programm
    {
        public static void Main(string[] args)
        {
            ServerHandler _server = new ServerHandler();
            Thread        _serverThread = new Thread(_server.run);

            _serverThread.Start();
        }
    }
}