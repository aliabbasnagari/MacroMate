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
                            await Dispatcher.DispatchAsync(async () =>
                            {
                                listener.Stop();
                                listener.Dispose();
                                lbStatus.Text = "Connected!";
                                await Navigation.PushAsync(new Profiles(editorIP.Text, int.Parse(editorPort.Text)));
                            });
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


        static byte[] ReadImage(string imagePath)
        {
            // Check if the file exists
            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Image file not found: {imagePath}");
                return null;
            }

            try
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        return br.ReadBytes((int)fs.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading image: {ex.Message}");
                return null;
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
