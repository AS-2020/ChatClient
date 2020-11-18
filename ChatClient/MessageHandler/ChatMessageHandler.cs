using ChatProtocol;
using System;
using System.Net.Sockets;

namespace ChatClient.MessageHandler
{
    public class ChatMessageHandler : IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message)
        {
            ChatMessage chatMessage = message as ChatMessage;
            Console.WriteLine($"Received: {chatMessage.Content}");
        }
    }
}
