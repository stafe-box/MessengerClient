using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClient
{
    public class Messanger
    {
        private string? _serverIP;

        public delegate void SendException(string e);
        public event SendException? SendExp;

        public delegate void Registered();
        public event Registered? RegSucsess;

        public delegate void RegistrationDenied();
        public event RegistrationDenied? RegFailed;

        public delegate void ServerNotRespoce();
        public event ServerNotRespoce? ServError;

        public delegate void MessageRecived(Message msg);
        public event MessageRecived? TextRecived;

        public delegate void GotWorkingException(string e);
        public event GotWorkingException? WorkingException;
        public void Send(string server, string message)
        {
            try
            {
                Int32 port = 13255;
                _serverIP = server;
                // Prefer a using declaration to ensure the instance is Disposed later.
                using TcpClient client = new TcpClient(_serverIP, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                this.MessagesWorker(responseData);
            }
            catch (ArgumentNullException e)
            {
                SendExp?.Invoke($"ArgumentNullException: {e}");
            }
            catch (SocketException e)
            {
                SendExp?.Invoke($"SocketException: {e}");
            }
        }
        private void Listen()
        {
            TcpListener? server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13255;
                IPAddress localAddr = IPAddress.Parse(CurrentUser.ServerIP);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[512];

                // Enter the listening loop.
                while (true)
                {
                    String? data = null;
                    using TcpClient client = server.AcceptTcpClient();

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                    }
                    if (data != null)
                    {
                        Task.Run(() =>
                        {
                            this.MessagesWorker(data);
                        });
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server?.Stop();
            }
        }
        private void MessagesWorker(string json)
        {
            try
            {
                Message msg = Message.Deserialize(json);
                if (msg.IsService)
                {
                    switch (msg.Text)
                    {
                        case "Registered":
                            CurrentUser.ServerIP = _serverIP;
                            RegSucsess?.Invoke();
                            Task.Run(() =>
                            {
                                this.Listen();
                            });
                            break;

                        case "Denied":
                            RegFailed?.Invoke();
                            break;

                        case "NewUser":
                            if (msg.Sender!= null) 
                                CurrentUser.Users.Add(msg.Sender);
                            break;

                        case "UserLeft":
                            if (msg.Sender != null)
                                CurrentUser.Users.Remove(msg.Sender);
                            break;
                        case "IsAlive":
                            Task.Run(() =>
                            {
                                this.Send(CurrentUser.ServerIP, "Alive");
                            });
                            break;

                        default:
                            ServError?.Invoke();
                            break;
                    }
                }
                else    //Text message block
                {
                    TextRecived?.Invoke(msg);
                }
            }
            catch (Exception e) 
            {
                WorkingException?.Invoke($"Got exceptiom while works on file:\n\t{e}");
            }
        }
    }
}
