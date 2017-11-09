using System;

namespace NetCoinche
{
    public struct MyIp
    {
        private string _ip;
        private string _port;

        public string Ip
        {
            get => _ip;
            set => _ip = value;
        }

        public string Port
        {
            get => _port;
            set => _port = value;
        }
        
        public static bool operator ==(MyIp c1, MyIp c2)
        {
            return (c1._ip.Equals(c2._ip) && c1._port.Equals(c2._port));
        }
        
        public static bool operator !=(MyIp c1, MyIp c2)
        {
            return (c1._ip != c2._ip || c1._port != c2._port);
        }
    }
    
}