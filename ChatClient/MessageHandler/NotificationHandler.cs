using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.MessageHandler
{
    class NotificationHandler : IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message)
        {
            ConnectNotification connectNotification = message as ConnectNotification;
            Console.Write($"{connectNotification.Name}: ");

        }
    }
}

