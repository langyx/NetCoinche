using System;
using System.Runtime.InteropServices;
using System.Threading;
using NetworkCommsDotNet;

namespace ClientApplication
{
    public class ConnectionObserver
    {
        private bool _run;
        
        public ConnectionObserver()
        {
            _run = true;
        }

        public void Run()
        {
            Console.WriteLine("Attemp connexion to " + Program.serverIP + ":" + Program.serverPort);
            while (_run)
            {
                try
                {
                    NetworkComms.SendObject("Connection", Program.serverIP, Program.serverPort, "a");
                }
                catch (ConnectionSetupException ex)
                {
                    Console.WriteLine("Error Connection to server !");
                    System.Environment.Exit(1);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine("Error communication with serveur !");
                    System.Environment.Exit(1);
                }
                catch (CommsException ex)
                {
                    Console.WriteLine("Erro from Commnetwork !");
                    System.Environment.Exit(1);
                }
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            _run = false;
        }

        public bool Run1
        {
            get => _run;
            set => _run = value;
        }
    }
}