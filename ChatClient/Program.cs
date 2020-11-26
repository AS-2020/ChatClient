using ChatClient.MessageHandler;
using ChatProtocol;
using ClientLibrary;
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
        
        public static string username;
        public static string password;
        public static bool IsApplicationExecuting = true;

        public static List<User> Users = new List<User>();

        public static Client Client;

        static void MessageHandle()
        {
            while (true)
            {
                lock (Client.ReceivedMessages)
                    foreach (var message in Client.ReceivedMessages)
                    {
                        MessageHandlerFactory.GetMessageHandler(message.MessageId).Execute(message);
                    }

                Client.ReceivedMessages.Clear();
            }
        }
        static void StartMessageHandleThread()
        {
            var messageHandleThreadStart = new ThreadStart(MessageHandle);
            var messageHandleThread = new Thread(messageHandleThreadStart);
            messageHandleThread.Start();
        }

        static void Main()
        {
            Client = new Client("127.0.0.1", 13000);
            StartMessageHandleThread();

            while (IsApplicationExecuting)
            {
                Console.Clear();

                Menu();

                while (Client.IsConnecting)
                {

                }

                while (Client.IsConnected)
                {
                    //Console.WriteLine("Nachricht eingeben:");
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        case "/disconnect":
                            Client.Disconnect();
                            break;
                        case "/exit":
                            IsApplicationExecuting = false;
                            Client.Disconnect();
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
                            Client.SendChatMessage(input, 0);
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
                Client.SendChatMessage(privatemessage, privateId);
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
                        Client.Connect(username, password);
                        test = false;
                        break;
                    case ConsoleKey.D2:
                        Console.Clear();
                        Console.Write("New Username: ");
                        username = Console.ReadLine();


                        Console.Write("New Password: ");
                        password = Console.ReadLine();

                        Client.Register(username, password);

                        test = false;
                        break;

                    default:
                        break;
                }
            } while (test);
        }
    }
}
