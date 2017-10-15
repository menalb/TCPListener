namespace TCPListener.Models
{
    class ResponseFromGateway
    {
        public ResponseFromGateway(string header, string message)
        {
            Header = header;
            Message = message;
        }

        public string Header { get; }
        public string Message { get; }
    }
}