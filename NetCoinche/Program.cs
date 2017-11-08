﻿using System;
using System.Collections.Generic;

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using static System.Int32;

namespace NetCoinche
{
    class Program
    {
        private static void Main(string[] args)
        {
            //Trigger the method PrintIncomingMessage when a packet of type 'Message' is received
            //We expect the incoming object to be a string which we state explicitly by using <string>
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", PrintIncomingMessage);
            //Start listening for incoming connections
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 4444));
 
            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);
 
            //Let the user close the server
            Console.WriteLine("\nPress any key to close server.");
            Console.ReadKey(true);
 
            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
        }
 
        /// <summary>
        /// Writes the provided message to the console window
        /// </summary>
        /// <param name="header">The packet header associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The message to be printed to the console</param>
        private static void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("\nA message was received from " + connection.ToString() + " which said '" + message + "'.");
            var adressClient = Tools.getIpPortFromString(connection.ConnectionInfo.RemoteEndPoint.ToString());
            NetworkComms.SendObject("Message", adressClient[0], Tools.getPortFromString(message), "nique ta mere");
        }
    }
}