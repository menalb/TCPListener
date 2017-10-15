using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using TCPListener.Models;

namespace TCPListener
{
    internal class Listener
    {
        private const int messageLength = 256;
        private readonly ClientSettings _clientSettings;
        private TcpClient clientSocket;

        public Listener(ClientSettings clientSettings)
        {
            _clientSettings = clientSettings;
        }

        public void Start(TcpClient inClientSocket)
        {
            clientSocket = inClientSocket;
            var handleThread = new Thread(Handle);

            handleThread.Start();
        }

        private void Handle()
        {
            byte[] bytes = new byte[messageLength];

            if (!clientSocket.Connected)
            {
                Log("Disconnected");
                return;
            }

            String data = null;
            NetworkStream stream = clientSocket.GetStream();
            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = Encoding.ASCII.GetString(bytes, 0, i);
                Log($"Received: {data}");

                var response = Process(stream, data);

                Log($"header: {response.Header}");
                Log($"message: {response.Message}");

                WriteReponse(stream, response);
            }

            // Shutdown and end connection
            clientSocket.Close();
        }

        private ResponseFromGateway Process(NetworkStream stream, string data)
        {
            // Process the data sent by the client.
            data = data.ToUpper();

            byte[] msg = Encoding.ASCII.GetBytes(data);

            var request = new RequestToProcess(Encoding.UTF8.GetString(msg));

            var _processor = CreateProcessor(_clientSettings);

            return _processor.GetResponse(request);                        
        }

        private void WriteReponse(NetworkStream stream, ResponseFromGateway response)
        {
            (var headerStream, var messageStream) = GetStream(response);

            // Send back a response.
            stream.Write(headerStream, 0, headerStream.Length);
            stream.Write(messageStream, 0, messageStream.Length);
        }

        private static (byte[] headerStream, byte[] messageStram) GetStream(ResponseFromGateway response)
        {
            return (Encoding.UTF8.GetBytes(response.Header), Encoding.UTF8.GetBytes(response.Message));
        }

        private static IProcessorGateway CreateProcessor(ClientSettings clientSettings)
        {
            return new ProcessorGateway(clientSettings);
        }

        private static void Log(string line)
        {
            Console.WriteLine($"SERVER> {line}");
        }
    }
}