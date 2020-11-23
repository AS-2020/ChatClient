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
            User user = Program.Users.Find(u => u.Id == chatMessage.UserId);
            string username = $"Unbekannt ({chatMessage.UserId})";
            if (user != null)
                username = user.Username;

            Console.WriteLine($"{username}: {chatMessage.Content}");
        }
    }
}
