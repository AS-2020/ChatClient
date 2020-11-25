using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace ClientLibrary
{
    public class Client
    {
        private string serverIpAddress;
        private int serverPort;

        private TcpClient client;
        public string SessionId;
        public bool IsConnected { get; private set; } = false;
        public bool IsConnecting { get; private set; } = false;

        public List<IMessage> ReceivedMessages = new List<IMessage>();
        Thread receiveDataThread;

        public Client(string ipAddress, int port)
        {
            serverIpAddress = ipAddress;
            serverPort = port;
        }
        public void ReceiveData()
        {
            while (client != null)
            {
                try
                {
                    lock (client)
                    {
                        var data = new byte[1024];
                        var bytes = client.GetStream().Read(data, 0, data.Length);
                        var responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                        var genericMessage = JsonSerializer.Deserialize<GenericMessage>(responseData);
                        var message = MessageFactory.GetMessage(genericMessage.MessageId, responseData);

                        // Connect response message received
                        if (message.MessageId == 4)
                            HandleConnectResponseMessage(message);

                        // Register response message received
                        if (message.MessageId == 8)
                            HandleRegisterResponseMessage(message);

                        lock (ReceivedMessages)
                            ReceivedMessages.Add(message);
                    }
                }
                catch (System.IO.IOException)
                { }
                catch (ObjectDisposedException)
                { }
            }
        }
        private void HandleRegisterResponseMessage(IMessage message)
        {
            var registerResponseMessage = message as RegisterResponseMessage;
            if (registerResponseMessage.Success)
            {
                IsConnected = true;
                SessionId = registerResponseMessage.SessionId;
            }

            IsConnecting = false;
        }

        private void HandleConnectResponseMessage(IMessage message)
        {
            var connectResponseMessage = message as ConnectResponseMessage;
            if (connectResponseMessage.Success)
            {
                IsConnected = true;
                SessionId = connectResponseMessage.SessionId;
            }

            IsConnecting = false;
        }

        private void StopReceiveDataThread()
        {
            // receiveDataThread.Interrupt();
            if (receiveDataThread != null)
                receiveDataThread.Join();
        }

        private void StartReceiveDataThread()
        {
            var threadStart = new ThreadStart(ReceiveData);
            receiveDataThread = new Thread(threadStart);
            receiveDataThread.Start();
        }

        
        public void Register(string username, string password)
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

        public void Connect(string username, string password)
        {
            IsConnecting = true;
            client = new TcpClient(serverIpAddress, serverPort);

            var connectMessage = new ConnectMessage
            {
                ServerPassword = "test123",
                Username = username,
                Password = password
            };

            StartReceiveDataThread();
            SendMessage(JsonSerializer.Serialize(connectMessage));
        }

        public void Disconnect()
        {
            var disconnectMessage = new DisconnectMessage()
            {
                SessionId = SessionId
            };
            SendMessage(JsonSerializer.Serialize(disconnectMessage));

            IsConnecting = false;
            IsConnected = false;
            SessionId = string.Empty;
            client.Close();
            client = null;
        }

        public void SendMessage(string messageJson)
        {
            // Verschlüsselung: messageJson verschlüsseln
            var data = System.Text.Encoding.UTF8.GetBytes(messageJson);
            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        public void SendChatMessage(string messageContent, int id)
        {
            try
            {
                // Prepare chat message
                var chatMessage = new ChatMessage
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

        public void RequestUserList()
        {
            var userListRequestMessage = new UserListRequestMessage
            {
                SessionId = SessionId
            };
            SendMessage(JsonSerializer.Serialize(userListRequestMessage));
        }
    }
}