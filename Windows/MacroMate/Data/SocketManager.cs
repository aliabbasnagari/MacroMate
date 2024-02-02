using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MacroMate.Data
{
    public class SocketManager
    {
        private static SocketManager? instance;
        private string ip;
        private int port;
        private TcpListener? listener;
        private TcpClient? client;

        private SocketManager()
        {
        }

        public static SocketManager GetInstance()
        {
            return instance ??= new SocketManager();
        }

        public void StartServer(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }
            try
            {
                listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                Debug.WriteLine($"Listening at {ip}:{port}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting server: {ex}");
            }
        }

        public async Task GetClient()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
            if (listener == null) return;
            Debug.WriteLine($"Waiting for client...");
            client = await listener.AcceptTcpClientAsync();
        }

        public async Task<bool> ConnectClient()
        {
            if (listener != null && client != null)
            {
                Debug.WriteLine($"Initiate Connection...");
                using (NetworkStream stream = client.GetStream())
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.WriteLine($"TCP RD: {message}");
                        if (message == "request")
                        {
                            byte[] responseBytes = Encoding.UTF8.GetBytes("connect");
                            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                        }
                        else if (message == "connected")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
