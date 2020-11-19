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
            {
                Program.IsConnected = true;
                Program.SessionId = connectResponseMessage.SessionId;
                Console.WriteLine($"Connected! Session Id: {Program.SessionId}");
            }
            else
            {
                Console.WriteLine("Connection failed!");
            }

            Program.IsConnecting = false;
        }
    }
}
