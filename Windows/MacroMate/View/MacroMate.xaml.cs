
using CommunityToolkit.Maui.Views;
using MacroMate.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
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
        private TcpListener listener;
        private ImageButton? selectedButton;
        private InputSimulator inputSimulator;
        private ProfileLayout? profileLayout;
        private Dictionary<string, VirtualKeyCode> keyboardKeys;
        private DatabaseContext db = DatabaseContext.getInstance();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MacroMate(string ip, int port, string profileName)
        {
            InitializeComponent();
            AddPickerOptions();

            this.ip = ip;
            this.port = port;
            this.profileName = profileName;
            lbProfile.Text = this.profileName;

            inputSimulator = new InputSimulator();
            listener = new TcpListener(IPAddress.Parse(this.ip), this.port);
            keyboardKeys = getKeyDict();
            if (db.profiles != null && db.profiles.ContainsKey(profileName))
                profileLayout = db.profiles[profileName];

            for (int i = 0; i < profileLayout.rows; i++) layoutGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            for (int i = 0; i < profileLayout.columns; i++) layoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            var get_icon = (File.Exists(profileLayout.icons["profile"]) ? profileLayout.icons["profile"] : "default_app_img.png");
            profileImage.Source = get_icon;

            int btnId = 5;
            for (int i = 0; i < profileLayout.rows; i++)
            {
                for (int j = 0; j < profileLayout.columns; j++)
                {
                    ImageButton imageBtn = new ImageButton();
                    imageBtn.ClassId = btnId.ToString();
                    imageBtn.Source = profileLayout.icons[btnId.ToString()];
                    imageBtn.WidthRequest = 100;
                    imageBtn.HeightRequest = 100;
                    imageBtn.BackgroundColor = Color.FromRgb(170, 217, 187);
                    imageBtn.Margin = new Thickness(7);
                    imageBtn.Clicked += OnGridBtnClick;

                    layoutGrid.Children.Add(imageBtn);
                    Grid.SetRow(imageBtn, i);
                    Grid.SetColumn(imageBtn, j);
                    btnId++;
                }
            }

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
            Dictionary<string, string> key_action = profileLayout.key_commands;
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

        private void AddPickerOptions()
        {
            pickerCommand1.Items.Add("-");
            pickerCommand1.Items.Add("ctrl");
            pickerCommand1.Items.Add("shift");
            pickerCommand1.Items.Add("alt");
            pickerCommand1.Items.Add("win");

            pickerCommand2.Items.Add("-");
            pickerCommand2.Items.Add("ctrl");
            pickerCommand2.Items.Add("shift");
            pickerCommand2.Items.Add("alt");
            pickerCommand2.Items.Add("win");

            pickerCommand3.Items.Add("-");
            for (char c = 'a'; c <= 'z'; c++) pickerCommand3.Items.Add(c.ToString());
            for (char c = '0'; c <= '9'; c++) pickerCommand3.Items.Add(c.ToString());
            for (int j = 1; j <= 12; j++) pickerCommand3.Items.Add($"F{j}");
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
            keyboardKeys.Add("win", VirtualKeyCode.LWIN);
            return keyboardKeys;
        }
        public void Dispose()
        {
            cancellationTokenSource.Dispose();
        }

        private void OnGridBtnClick(object sender, EventArgs e)
        {
            if (selectedButton != null) selectedButton.BackgroundColor = Color.FromRgb(170, 217, 187);

            ImageButton btn = (ImageButton)sender;
            btn.BackgroundColor = Color.FromRgb(128, 188, 189);
            string[] keys = profileLayout.key_commands[btn.ClassId].Split("+");
            if (keys.Length == 3)
            {
                pickerCommand1.SelectedItem = keys[0];
                pickerCommand2.SelectedItem = keys[1];
                pickerCommand3.SelectedItem = keys[2];
            }
            selectedButton = btn;
        }

        private async void profileImageChoose(object sender, EventArgs e)
        {
            var result = await this.ShowPopupAsync(new ChooseImage());
            if (result != null)
            {
                string newval = $"{result}";
                if (profileLayout != null && profileLayout.icons["profile"] != newval)
                {
                    profileLayout.icons["profile"] = newval;
                    profileImage.Source = newval;
                    db.UpdateProfiles();
                    await DisplayAlert("Update", "Done", "Ok");
                }
            }
        }

        private void btnAssign_Clicked(object sender, EventArgs e)
        {
            if (profileLayout != null)
            {
                string new_val = $"{pickerCommand1.SelectedItem}+{pickerCommand2.SelectedItem}+{pickerCommand3.SelectedItem}";
                string new_path = selectedButton?.Source?.ToString()?.Replace("File:", "").Trim() ?? "default_btn_img.png";
                if (selectedButton != null && (profileLayout.key_commands[selectedButton.ClassId] != new_val || profileLayout.icons["profile"] != new_path))
                {
                    profileLayout.key_commands[selectedButton.ClassId] = new_val;
                    profileLayout.icons[selectedButton.ClassId] = new_path;
                    db.UpdateProfiles();
                    DisplayAlert("Update", "Done", "Ok");
                }
            }
        }

        private async void btnIconSelect_Clicked(object sender, EventArgs e)
        {
            var result = await this.ShowPopupAsync(new ChooseImage());
            if (result != null && selectedButton != null)
            {
                selectedButton.Source = result.ToString();
            }
        }
    }
}
