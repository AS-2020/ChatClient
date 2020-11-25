using ChatProtocol;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;

namespace ChatClient.MessageHandler
{
    public class UserListResponseMessageHandler : IMessageHandler
    {
        public void Execute(IMessage message)
        {
            UserListResponseMessage userListResponseMessage = message as UserListResponseMessage;
            Program.Users = JsonSerializer.Deserialize<List<User>>(userListResponseMessage.UserListJson);
        }
    }
}
