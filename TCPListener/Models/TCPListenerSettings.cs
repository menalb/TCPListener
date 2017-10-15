namespace TCPListener.Models
{
    internal class ListenerSettings
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }

    internal class ClientSettings
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }

    internal class TCPListenerSettings
    {
        public ClientSettings ClientSettings { get; set; }
        public ListenerSettings ListenerSettings { get; set; }
    }
}