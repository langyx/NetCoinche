using System;
using System.Linq;
using static System.Int32;

namespace NetCoinche
{
    public static class Tools
    {
        public static string[] getIpPortFromString(string str)
        {
            var ipAndPort = new string[2];

            var serverIP = str.Split(':').First();
            var serverPort = str.Split(':').Last();
           
            ipAndPort[0] = serverIP;
            ipAndPort[1] = serverPort;
            
            return ipAndPort;
        }

        public static int getPortFromString(string port)
        {
            return Parse(port.Split(':').Last());
        }
    }
}