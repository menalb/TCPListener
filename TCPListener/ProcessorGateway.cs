using System.Net.Sockets;
using System.Text;

using TCPListener.Models;

namespace TCPListener
{
    internal class ProcessorGateway : IProcessorGateway
    {
        private const int headerLength = 18;

        private readonly ClientSettings _clientSettings;
        private readonly TcpClient _remoteSocket;
        private NetworkStream _networkStream;        

        public ProcessorGateway(ClientSettings clientSettings)
        {
            _remoteSocket = new TcpClient();
            _clientSettings = clientSettings;
        }

        public ResponseFromGateway GetResponse(RequestToProcess request)
        {
            _remoteSocket.Connect(_clientSettings.IPAddress, _clientSettings.Port);

            _networkStream = _remoteSocket.GetStream();

            Send(request.Message);

            var header = Read(headerLength);

            var messageLength = GetMessageLength(header);
            var message = Read(messageLength);

            var response = new ResponseFromGateway(header, message);

            _remoteSocket.Close();
            return response;
        }

        private int GetMessageLength(string header)
        {
            return int.Parse(header.Substring(2, 8).ToString());
        }

        private void Send(string message)
        {
            byte[] converted = Encoding.UTF8.GetBytes(message);
            _networkStream.Write(converted, 0, converted.Length);
        }

        private string Read(int lengthToread)
        {
            var networkStream = _remoteSocket.GetStream();
            byte[] buffer = new byte[lengthToread];
            var responseString = new StringBuilder();
            int bytesRemaining = lengthToread;
            int bytesRead = 0;

            while (bytesRemaining > 0)
            {
                bytesRead = networkStream.Read(buffer, 0, bytesRemaining);
                responseString.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                bytesRemaining -= bytesRead;
            }
            return responseString.ToString();
        }
    }

    internal interface IProcessorGateway
    {
        ResponseFromGateway GetResponse(RequestToProcess request);
    }
}