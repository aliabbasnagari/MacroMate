
using MacroMate.Data;
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
        private string profileName;
        private int numButtons;
        private TcpListener listener;
        private InputSimulator inputSimulator;
        private Dictionary<string, VirtualKeyCode> keyboardKeys;
        private DatabaseContext db = DatabaseContext.getInstance();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MacroMate(string ip, int port, string profileName)
        {
            this.ip = ip;
            this.port = port;
            this.profileName = profileName;
            inputSimulator = new InputSimulator();
            keyboardKeys = getKeyDict();
            listener = new TcpListener(IPAddress.Parse(this.ip), port);
            InitializeComponent();
            Task.Run(() => StartServerAsync(cancellationTokenSource.Token));
            lbProfile.Text = this.profileName;
            switch (db.profiles[profileName].layout_index)
            {
                case "1":
                    imgLayout.Source = "ml1.png"; numButtons = 8; break;
                case "2":
                    imgLayout.Source = "ml2.png"; numButtons = 10; break;
                case "3":
                    imgLayout.Source = "ml3.png"; numButtons = 12; break;
                case "4":
                    imgLayout.Source = "ml4.png"; numButtons = 18; break;
            }

            foreach (var pair in db.profiles[profileName].key_commands)
            {
                string key = pair.Key;
                string[] value = pair.Value.Split("+");
                if (int.Parse(key) < 3)
                {

                }
                else if (value.Length == 3)
                {
                    HorizontalStackLayout hzSL = new HorizontalStackLayout();
                    Label lbl = new Label();
                    lbl.Text = $"Button {key}";
                    lbl.VerticalOptions = LayoutOptions.Center;
                    lbl.Margin = new Thickness(0, 0, 20, 0);
                    Picker picker1 = new Picker();
                    picker1.WidthRequest = 125;
                    picker1.Margin = new Thickness(0, 0, 20, 0);
                    picker1.Items.Add("-");
                    picker1.Items.Add("ctrl");
                    picker1.Items.Add("shift");
                    picker1.Items.Add("alt");
                    picker1.SelectedItem = value[0];

                    Picker picker2 = new Picker();
                    picker2.WidthRequest = 125;
                    picker2.Margin = new Thickness(0, 0, 20, 0);
                    picker2.Items.Add("-");
                    picker2.Items.Add("ctrl");
                    picker2.Items.Add("shift");
                    picker2.Items.Add("alt");
                    picker2.SelectedItem = value[1];

                    Picker picker3 = new Picker();
                    picker3.WidthRequest = 125;
                    picker3.Margin = new Thickness(0, 0, 20, 0);
                    picker3.Items.Add("-");
                    for (char c = 'a'; c <= 'z'; c++) picker3.Items.Add(c.ToString());
                    for (char c = '0'; c <= '9'; c++) picker3.Items.Add(c.ToString());
                    for (int j = 1; j <= 12; j++) picker3.Items.Add($"F{j}");
                    picker3.SelectedItem = value[2];

                    hzSL.Children.Add(lbl);
                    hzSL.Children.Add(picker1);
                    hzSL.Children.Add(picker2);
                    hzSL.Children.Add(picker3);
                    hzSL.HorizontalOptions = LayoutOptions.Center;
                    verticalSL.Children.Add(hzSL);
                }
            }
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
            Dictionary<string, string> key_action = db.profiles[profileName].key_commands;
            if (key_action != null && key_action.ContainsKey(message))
            {
                string[] keys = key_action[message].Split("+").Where(item => item != "-").ToArray();
                if (keys.Length == 1 && keyboardKeys.TryGetValue(keys[0], out var k1))
                {
                    inputSimulator.Keyboard.KeyPress(k1);
                }
                else if (keys.Length == 2 && keyboardKeys.TryGetValue(keys[0], out var k2) && keyboardKeys.TryGetValue(keys[1], out var k3))
                {
                    inputSimulator.Keyboard.ModifiedKeyStroke(k2, k3);
                }
                else if (keys.Length == 3 && keyboardKeys.TryGetValue(keys[0], out var k4) && keyboardKeys.TryGetValue(keys[1], out var k5) && keyboardKeys.TryGetValue(keys[2], out var k6))
                {   
                    inputSimulator.Keyboard.ModifiedKeyStroke(new List<VirtualKeyCode> { k4, k5 }, k6);
                }
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
