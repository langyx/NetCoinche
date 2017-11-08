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
    }
}