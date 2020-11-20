using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.MessageHandler
{
    public class RegisterResponseMessageHandler : IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message)
        {
            RegisterResponseMessage registerResponseMessage = message as RegisterResponseMessage;

            if (registerResponseMessage.Success)
            {
                Program.IsConnected = true;
                Program.SessionId = registerResponseMessage.SessionId;
                Console.WriteLine($"Connected! Session Id: {Program.SessionId}");
            }

            Console.WriteLine($"{registerResponseMessage.Content}");

            Program.IsConnecting = false;
        }
    }
}
