namespace ServerC2
{

    public class SocketSettings
    {
        public SocketSettings()
        {
            IpAddress = "127.0.0.1";
            Port = "2222";
        }

        public string IpAddress { get; set; }

        public string Port { get; set; }
    }
}