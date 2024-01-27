using Microsoft.Maui.Controls.Platform;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MacroMate.View;
using MacroMate.Data;

namespace MacroMate
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            editorIP.Text = GetLocalIpAddress();
            editorPort.Text = "7214";
        }

        private void StartConnection(object sender, EventArgs e)
        {
            lbStatus.Text = "Listening...";
            lbStatus.TextColor = Colors.Orange;
            Task.Run(ConnectToServer);
        }

        private async Task ConnectToServer()
        {
            using (TcpListener listener = new TcpListener(IPAddress.Parse(editorIP.Text), int.Parse(editorPort.Text)))
            {
                try
                {
                    if (!IPAddress.TryParse(editorIP.Text, out IPAddress? ipAddress) || !int.TryParse(editorPort.Text, out int port))
                    {
                        Dispatcher.Dispatch(() => lbStatus.Text = "Invalid IP or Port!");
                        return;
                    }
                    listener.Start();
                    while (true)
                    {
                        using (TcpClient client = await listener.AcceptTcpClientAsync())
                        using (NetworkStream stream = client.GetStream())
                        {
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
                                await Dispatcher.DispatchAsync(async () =>
                                {
                                    listener.Stop();
                                    listener.Dispose();
                                    lbStatus.Text = "Connected!";
                                    await Navigation.PushAsync(new Profiles(editorIP.Text, int.Parse(editorPort.Text)));
                                });
                                break;
                            }
                        }
                    }
                    listener.Stop();
                    listener.Dispose();
                }
                catch (Exception ex)
                {
                    Dispatcher.Dispatch(() => DisplayAlert("Exception", ex.Message, "OK"));
                }
            }
        }

        private string GetLocalIpAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
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
