using ChatClient.MessageHandler;
using ChatProtocol;
using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static string serverIpAddress = "127.0.0.1";
        static int serverPort = 13000;

        static TcpClient client;

        static void Connect()
        {
            client = new TcpClient(serverIpAddress, serverPort);

            ThreadStart threadStart = new ThreadStart(ReceiveData);
            Thread thread = new Thread(threadStart);
            thread.Start();

            ConnectMessage connectMessage = new ConnectMessage();
            connectMessage.ServerPassword = "test123";
            connectMessage.ClientName = "PeterParker";
            connectMessage.ClientPassword = "123";
            SendMessage(JsonSerializer.Serialize(connectMessage));
        }

        static void SendMessage(string messageJson)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(messageJson);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        static void ReceiveData()
        {
            while (true)
            {
                byte[] data = new byte[256];
                int bytes = client.GetStream().Read(data, 0, data.Length);
                string responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                GenericMessage genericMessage = JsonSerializer.Deserialize<GenericMessage>(responseData);
                IMessage message = MessageFactory.GetMessage(genericMessage.MessageId, responseData);
                IMessageHandler messageHandler = MessageHandlerFactory.GetMessageHandler(genericMessage.MessageId);
                messageHandler.Execute(client, message);
            }
        }

        static void SendChatMessage(string messageContent)
        {
            try
            {
                // Prepare chat message
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.Content = messageContent;

                // Send message
                SendMessage(JsonSerializer.Serialize(chatMessage));
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        static void Main()
        {
            Connect();

            while (true)
            {
                Console.WriteLine("Nachricht eingeben:");
                string input = Console.ReadLine();
                SendChatMessage(input);
            }

            client.Close();
        }
    }
}
