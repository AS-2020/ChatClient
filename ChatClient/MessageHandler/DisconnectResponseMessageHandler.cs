using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.MessageHandler
{
    class DisconnectResponseMessageHandler : IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message)
        {
            DisconnectResponseMessage disconnectResponseMessage = message as DisconnectResponseMessage;

            Console.WriteLine($"User {disconnectResponseMessage.Username} disconnected");
        }
    }
}
