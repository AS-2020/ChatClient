using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatClient.MessageHandler
{
    public class RegisterResponseMessageHandler : IMessageHandler
    {
        public void Execute(IMessage message)
        {
            RegisterResponseMessage registerResponseMessage = message as RegisterResponseMessage;

            if (registerResponseMessage.Success)
            {
                Program.Client.SessionId = registerResponseMessage.SessionId;
                Console.WriteLine($"Connected! Session Id: {Program.Client.SessionId}");
            }

            Console.WriteLine($"{registerResponseMessage.Content}");

        }
    }
}
