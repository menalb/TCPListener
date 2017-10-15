using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using TCPListener.Models;

namespace TCPListener
{
    class Program
    {
        static void Main(string[] args)
        {

            var configuration = GetConfiguration();

            var serverSocket = new TcpListener(IPAddress.Parse(configuration.ListenerSettings.IPAddress), configuration.ListenerSettings.Port);

            serverSocket.Start();
            Console.WriteLine("Listener started");

            while (true)
            {
                TcpClient clientSocket = serverSocket.AcceptTcpClient();
                Listener listener = new Listener(configuration.ClientSettings);
                listener.Start(clientSocket);
            }
        }

        private static TCPListenerSettings GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            
            var listenerSettings = new ListenerSettings();
            configuration.GetSection("ListenerSettings").Bind(listenerSettings);

            var clientSettings = new ClientSettings();
            configuration.GetSection("ClientSettings").Bind(clientSettings);

            return new TCPListenerSettings
            {                
                ListenerSettings = listenerSettings,
                ClientSettings = clientSettings
            };
        }
    }
}