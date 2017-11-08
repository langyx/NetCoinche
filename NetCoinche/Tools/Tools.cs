using System;
using System.Linq;
using static System.Int32;

namespace NetCoinche
{
    public static class Tools
    {
        public static MyIp getIpPortFromString(string str)
        {
            var newIp = new MyIp();

            var serverIP = str.Split(':').First();
            var serverPort = str.Split(':').Last();
         
            newIp.Ip = serverIP;
            newIp.Port = serverPort;
            
            return newIp;
        }

        public static int getPortFromString(string port)
        {
            return Parse(port.Split(':').Last());
        }

        public static string getMessageFromClient(string message)
        {
            return message.Split(':').First();
        }
    }
}