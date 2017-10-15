namespace TCPListener.Models
{
    public class RequestToProcess
    {
        public RequestToProcess(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}