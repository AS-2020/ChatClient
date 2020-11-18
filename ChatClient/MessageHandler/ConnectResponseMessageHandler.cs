using ChatProtocol;
using System;
using System.Net.Sockets;

namespace ChatClient.MessageHandler
{
    public class ConnectResponseMessageHandler : IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message)
        {
            ConnectResponseMessage connectResponseMessage = message as ConnectResponseMessage;
            if (connectResponseMessage.Success)
                Console.WriteLine($"Connected to {connectResponseMessage.Zaehler}!");
            else
                Console.WriteLine("Connection failed!");
        }
    }
}
