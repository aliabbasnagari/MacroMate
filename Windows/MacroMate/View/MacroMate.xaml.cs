using System.Net;
using System.Net.Sockets;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace MacroMate.View
{
    public partial class MacroMate : ContentPage, IDisposable
    {
        private string ip;
        private int port;
        private TcpListener listener;
        private InputSimulator inputSimulator;
        Dictionary<string, VirtualKeyCode> keyboardKeys;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MacroMate(string ip, int port, string profileName)
        {
            this.ip = ip;
            this.port = port;
            inputSimulator = new InputSimulator();
            keyboardKeys = getKeyDict();
            listener = new TcpListener(IPAddress.Parse(this.ip), port);
            InitializeComponent();
            Task.Run(() => StartServerAsync(cancellationTokenSource.Token));
        }

        private async Task StartServerAsync(CancellationToken cancellationToken)
        {
            try
            {
                listener.Start();
                while (!cancellationToken.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (message.Length > 0) SimulateKey(message);
                    stream.Flush();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                Dispatcher.Dispatch(() => DisplayAlert("Exception", ex.Message, "OK"));
            }
            finally
            {
                listener.Stop();
                listener.Server.Dispose();
            }
        }

        private void SimulateKey(string message)
        {
            var keyMappings = new Dictionary<string, VirtualKeyCode>
            {
                {"00", VirtualKeyCode.VK_A},
                {"01", VirtualKeyCode.VK_B},
                {"02", VirtualKeyCode.VK_C},
                {"03", VirtualKeyCode.VK_D},
                {"04", VirtualKeyCode.VK_E},
                {"10", VirtualKeyCode.VK_F},
                {"11", VirtualKeyCode.VK_G},
                {"12", VirtualKeyCode.VK_H},
                {"13", VirtualKeyCode.VK_I},
                {"14", VirtualKeyCode.VK_J},
            };

            if (keyMappings.TryGetValue(message, out var keyCode))
            {
                inputSimulator.Keyboard.KeyPress(keyCode);
            }
        }

        protected override void OnDisappearing()
        {
            cancellationTokenSource.Cancel();
            listener?.Stop();
            Navigation.PopAsync();
            base.OnDisappearing();
        }

        private Dictionary<string, VirtualKeyCode> getKeyDict()
        {
            Dictionary<string, VirtualKeyCode> keyboardKeys = new Dictionary<string, VirtualKeyCode>();
            // Add alphabetical keys
            for (char c = 'A'; c <= 'Z'; c++)
            {
                string key = c.ToString().ToLower();
                keyboardKeys.Add(key, (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), $"VK_{c}"));
            }

            // Add numerical keys
            for (int i = 0; i <= 9; i++)
            {
                string key = i.ToString();
                keyboardKeys.Add(key, (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), $"VK_{key}"));
            }

            // Add function keys
            for (int i = 1; i <= 12; i++)
            {
                string key = $"F{i}";
                keyboardKeys.Add(key, (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), key));
            }

            // Add common special keys
            keyboardKeys.Add("enter", VirtualKeyCode.RETURN);
            keyboardKeys.Add("space", VirtualKeyCode.SPACE);
            keyboardKeys.Add("backspace", VirtualKeyCode.BACK);
            keyboardKeys.Add("tab", VirtualKeyCode.TAB);
            keyboardKeys.Add("shift", VirtualKeyCode.SHIFT);
            keyboardKeys.Add("ctrl", VirtualKeyCode.CONTROL);
            keyboardKeys.Add("alt", VirtualKeyCode.MENU);
            keyboardKeys.Add("esc", VirtualKeyCode.ESCAPE);
            keyboardKeys.Add("capslock", VirtualKeyCode.CAPITAL);
            return keyboardKeys;
        }

        public void Dispose()
        {
            cancellationTokenSource.Dispose();
        }
    }
}
