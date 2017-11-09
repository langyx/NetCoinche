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

        public static int RandomInt(int min, int max)
        {
            var rnd = new Random();
            return rnd.Next(min, max);
        }

        public static int CountArray(object[] data)
        {
            var count = 0;

            for (var i = 0; i < data.Length; i += 1)
            {
                if (data[i] != null)
                {
                    count += 1;
                }
            }
            return count;
        }

        public static bool equalsIgnoreCase(this string str, string compStr)
        {
            return str.ToLower().Equals(compStr.ToLower());
        }
        
        public static string randomPlayerName()
        {
            string newName = "player";
            newName += Tools.RandomInt(0, 20000);
            return newName;
        }
        
        public static int tryParse(string text) {
            int myInt = int.TryParse(text, out myInt) ? myInt : -1;
            return myInt;
        }
    }
}