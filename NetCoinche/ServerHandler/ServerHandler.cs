using System;
using System.Collections.Generic;

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
        private bool _running;

        public bool Running
        {
            get => _running;
            set => _running = value;
        }

        public ServerHandler()
        {
            _running = true;
        }

        public void run()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", ReceiveMessage);
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 4444));
            while (_running){}
            NetworkComms.Shutdown();
        }
 
        private void ReceiveMessage(PacketHeader header, Connection connection, string message)
        {
           var adressClient = Tools.getIpPortFromString(connection.ConnectionInfo.RemoteEndPoint.ToString());
           this.ManageCommand(adressClient, Tools.getMessageFromClient(message));
        }

        private void ManageCommand(MyIp clientIp, string message)
        {
           Console.WriteLine(message);
            /*  NetworkComms.SendObject("Message", adressClient[0], Tools.getPortFromString(message), "nique ta mere");*/
        }
    }
}