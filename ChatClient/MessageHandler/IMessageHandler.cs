using ChatProtocol;
using System.Net.Sockets;

namespace ChatClient.MessageHandler
{
    public interface IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message);
    }
}
