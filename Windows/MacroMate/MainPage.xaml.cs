using Microsoft.Maui.Controls.Platform;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MacroMate.View;

namespace MacroMate
{
    public partial class MainPage : ContentPage
    {

        private TcpListener? listener;

        public MainPage()
        {
            InitializeComponent();
            editorIP.Text = GetLocalIpAddress();
            editorPort.Text = "7214";
        }

        private void StartConnection(object sender, EventArgs e)
        {
            listener = new TcpListener(IPAddress.Parse(editorIP.Text), int.Parse(editorPort.Text));
            lbStatus.Text = "Listening...";
            lbStatus.TextColor = Colors.Orange;
            Task.Run(StartServer);
        }

        private async Task StartServer()
        {
            if (listener != null)
            {
                try
                {
                    listener.Start();
                    while (true)
                    {
                        TcpClient client = await listener.AcceptTcpClientAsync();
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[1024];

                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        if (message == "request")
                        {
                            byte[] responseBytes = Encoding.UTF8.GetBytes("connect");
                            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                        }
                        else if (message == "connected")
                        {
                            lbStatus.Dispatcher.Dispatch(() => lbStatus.Text = "Connected!");
                            Dispatcher.Dispatch(() => Navigation.PushAsync(new Profiles(editorIP.Text, int.Parse(editorPort.Text))));
                            break;
                        }

                        stream.Flush();
                        client.Close();
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Dispatch(() => DisplayAlert("Exception", ex.Message, "OK"));
                }
                finally
                {
                    listener.Stop();
                    listener.Dispose();
                }
            }
        }


        private string GetLocalIpAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
            return "IP address not found";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            lbStatus.Text = "Not Connected!";
            lbStatus.TextColor = Colors.IndianRed;
        }

    }

}
