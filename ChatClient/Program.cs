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
        public static string SessionId;
        public static bool IsConnected = false;
        public static bool IsConnecting = false;

        static void Connect(string username, string password)
        {
            IsConnecting = true;
            client = new TcpClient(serverIpAddress, serverPort);

            ConnectMessage connectMessage = new ConnectMessage
            {
                ServerPassword = "test123",
                Username = username,
                Password = password
            };

            StartReceiveDataThread();
            SendMessage(JsonSerializer.Serialize(connectMessage));
        }
        public static void StartReceiveDataThread()
        {
            ThreadStart threadStart = new ThreadStart(ReceiveData);
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

        static void Notificate() // löschen?
        {
            client = new TcpClient(serverIpAddress, serverPort);

            ThreadStart threadStart = new ThreadStart(ReceiveData);
            Thread thread = new Thread(threadStart);
            thread.Start();

            ConnectNotification connectNotification = new ConnectNotification
            {
                Name = "PeterParker"
            };
            SendMessage(JsonSerializer.Serialize(connectNotification));
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
                try
                {
                    byte[] data = new byte[256];
                    int bytes = client.GetStream().Read(data, 0, data.Length);
                    string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    GenericMessage genericMessage = JsonSerializer.Deserialize<GenericMessage>(responseData);
                    IMessage message = MessageFactory.GetMessage(genericMessage.MessageId, responseData);
                    IMessageHandler messageHandler = MessageHandlerFactory.GetMessageHandler(genericMessage.MessageId);
                    messageHandler.Execute(client, message);
                }
                catch (System.IO.IOException)
                { }
                catch (System.ObjectDisposedException)
                { }
            }
        }

        static void SendChatMessage(string messageContent)
        {
            try
            {
                // Prepare chat message
                ChatMessage chatMessage = new ChatMessage
                {
                    Content = messageContent,
                    SessionId = SessionId
                };

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
            Console.WriteLine("Username: ");
            string username = Console.ReadLine();


            Console.WriteLine("Password: ");
            string password = Console.ReadLine();

            Console.WriteLine("Connecting to server.");
            Connect(username, password);

            // Notificate(); 

            while (IsConnecting)
            {

            }

            while (IsConnected)
            {
                //Console.WriteLine("Nachricht eingeben:");
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    IsConnected = false;
                    DisconnectMessage disconnectMessage = new DisconnectMessage
                    {
                        Username = username,
                        SessionId = SessionId
                    };
                    SendMessage(JsonSerializer.Serialize(disconnectMessage));
                }
                else
                {
                    SendChatMessage(input);
                }
            }

            client.Close();
            Console.WriteLine("Connection closed.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
