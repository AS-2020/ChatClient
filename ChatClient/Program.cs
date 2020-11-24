using ChatClient.MessageHandler;
using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static string username;
        public static string password;
        public static Thread receiveDataThread;
        public static bool IsApplicationExecuting = true;

        public static List<User> Users = new List<User>();


        public static void RequestUserList()
        {
            UserListRequestMessage userListRequestMessage = new UserListRequestMessage
            {
                SessionId = SessionId
            };
            SendMessage(JsonSerializer.Serialize(userListRequestMessage));
        }
        static void Disconnect()
        {
            DisconnectMessage disconnectMessage = new DisconnectMessage()
            {
                SessionId = SessionId
            };
            SendMessage(JsonSerializer.Serialize(disconnectMessage));

            StopReceiveDataThread();

            IsConnecting = false;
            IsConnected = false;
            SessionId = string.Empty;
            client.Close();
            client = null;
        }

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
        static void Register(string username, string password)
        {
            IsConnecting = true;

            client = new TcpClient(serverIpAddress, serverPort);

            RegisterMessage registerMessage = new RegisterMessage
            {
                ServerPassword = "test123",
                Username = username,
                Password = password
            };

            StartReceiveDataThread();
            SendMessage(JsonSerializer.Serialize(registerMessage));
        }
        public static void StartReceiveDataThread()
        {
            ThreadStart threadStart = new ThreadStart(ReceiveData);
            receiveDataThread = new Thread(threadStart);
            receiveDataThread.Start();
        }
        public static void StopReceiveDataThread()
        {
            receiveDataThread.Interrupt();
            //if (receiveDataThread != null)
            //    receiveDataThread.Join();
        }


        static void SendMessage(string messageJson)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(messageJson);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        static void ReceiveData()
        {
            while (client != null)
            {
                try
                {
                    lock (client)
                    {
                        byte[] data = new byte[1024];
                        int bytes = client.GetStream().Read(data, 0, data.Length);
                        string responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                        GenericMessage genericMessage = JsonSerializer.Deserialize<GenericMessage>(responseData);
                        IMessage message = MessageFactory.GetMessage(genericMessage.MessageId, responseData);
                        IMessageHandler messageHandler = MessageHandlerFactory.GetMessageHandler(genericMessage.MessageId);
                        messageHandler.Execute(client, message);
                    }
                }
                catch (System.IO.IOException)
                { }
                catch (System.ObjectDisposedException)
                { }
            }
        }

        static void SendChatMessage(string messageContent, int id)
        {
            try
            {
                // Prepare chat message
                ChatMessage chatMessage = new ChatMessage
                {
                    Content = messageContent,
                    SessionId = SessionId,
                    PrivateId = id
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
            while (IsApplicationExecuting)
            {
                Console.Clear();

                Menu();

                while (IsConnecting)
                {

                }

                while (IsConnected)
                {
                    //Console.WriteLine("Nachricht eingeben:");
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        case "/disconnect":
                            Disconnect();
                            break;
                        case "/exit":
                            Disconnect();
                            IsApplicationExecuting = false;
                            break;
                        case "/users":
                            PrintUsers();
                            break;
                        case "/user":
                            GetUserId();
                            break;
                        case "/message":
                            PrivateMessage();
                            break;
                        default:
                            SendChatMessage(input, 0);
                            break;
                    }
                }
            }
        }

        static void PrivateMessage()
        {
            Console.WriteLine("Enter UserId!");            
            if (int.TryParse(Console.ReadLine(), out int privateId))
            {
                Console.WriteLine("Enter private Message!");
                string privatemessage = Console.ReadLine();
                SendChatMessage(privatemessage, privateId);
            }

        }

        static void GetUserId()
        {
            Console.WriteLine("Enter Username!");
            string searchuser = Console.ReadLine();
            var queruser = from u in Users where u.Username.ToLower().Contains(searchuser.ToLower()) select u;
            foreach (User u in queruser)
            {
                Console.WriteLine(u);
            }
        }
        static void PrintUsers()
        {
            foreach (User u in Users)
            {
                Console.WriteLine(u);
            }
        }

        static void Menu()
        {
            bool test = true;
            do
            {
                Console.Clear();
                Console.WriteLine("(1)  Anmelden");
                Console.WriteLine("(2)  Neuen Benutzer registrieren");
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        Console.Clear();
                        Console.Write("Username: ");
                        username = Console.ReadLine();

                        Console.Write("Password: ");
                        password = Console.ReadLine();

                        Console.WriteLine("Connecting to server.");
                        Connect(username, password);
                        test = false;
                        break;
                    case ConsoleKey.D2:
                        Console.Clear();
                        Console.Write("New Username: ");
                        username = Console.ReadLine();


                        Console.Write("New Password: ");
                        password = Console.ReadLine();

                        Register(username, password);

                        test = false;
                        break;

                    default:
                        break;
                }
            } while (test);
        }
    }
}
