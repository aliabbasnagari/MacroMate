using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MacroMate.Data;
namespace MacroMate.View;

public partial class Profiles : ContentPage
{
    private string ip;
    private int port;
    private DatabaseContext db = DatabaseContext.getInstance();
    public Profiles(string ip, int port)
    {
        InitializeComponent();
        this.ip = ip;
        this.port = port;
        InitLayout();
    }

    private void InitLayout()
    {
        var tapGes = new TapGestureRecognizer();
        tapGes.Tapped += Profile_Clicked;
        foreach (var prof in db.profiles)
        {
            var get_icon = (File.Exists(prof.Value.profile_icon) ? prof.Value.profile_icon : "default_app_img.png");
            Microsoft.Maui.Controls.Image img = new Microsoft.Maui.Controls.Image();
            img.WidthRequest = 60;
            img.HeightRequest = 60;
            img.Source = get_icon;

            Label lbl = new Label();
            lbl.HorizontalOptions = LayoutOptions.Center;
            lbl.Text = $"{prof.Key}";
            lbl.FontSize = 18;

            VerticalStackLayout vsl = new VerticalStackLayout();
            vsl.WidthRequest = 200;
            vsl.HeightRequest = 100;
            vsl.BackgroundColor = Colors.Gray;
            vsl.ClassId = $"{prof.Key}";
            vsl.Margin = new Thickness(7);
            vsl.Children.Add(img);
            vsl.Children.Add(lbl);
            vsl.Spacing = 7;
            vsl.HorizontalOptions = LayoutOptions.Center;
            vsl.VerticalOptions = LayoutOptions.Center;
            vsl.GestureRecognizers.Add(tapGes);

            vsLayout.Children.Add(vsl);
        }
    }

    private async void Profile_Clicked(object sender, EventArgs e)
    {
        VerticalStackLayout btn = (VerticalStackLayout)sender;
        if (db.profiles != null && db.profiles.ContainsKey(btn.ClassId))
        {
            await StartServer(db.profiles[btn.ClassId]);
            await Navigation.PushAsync(new MacroMate(ip, port, btn.ClassId));
        }
    }

    private void btnCreate_Clicked(object sender, EventArgs e)
    {
    }

    private async Task StartServer(ProfileLayout profile)
    {
        try
        {
            using (TcpListener server = new TcpListener(IPAddress.Parse(ip), port))
            {
                server.Start();

                using (TcpClient client = await server.AcceptTcpClientAsync())
                {
                    await SendImagesAsync(client, profile.icons, profile.rows, profile.columns);
                }
            }
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(() => DisplayAlert("Here 1", ex.Message, "ok"));
        }
    }
  
    private async Task SendImagesAsync(TcpClient client, Dictionary<string, string> icons, int rows, int cols)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            using (var streamer = new BufferedStream(stream))
            {
                byte[] lengthBytes = BitConverter.GetBytes(rows);
                Array.Reverse(lengthBytes);
                await streamer.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                lengthBytes = BitConverter.GetBytes(cols);
                Array.Reverse(lengthBytes);
                await streamer.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                foreach (var img in icons)
                {
                    byte[] file = ResizeImage(img.Value, 128,128);//File.ReadAllBytes(img.Value);
                    lengthBytes = BitConverter.GetBytes(file.Length);
                    Array.Reverse(lengthBytes);
                    await streamer.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                    await streamer.WriteAsync(file, 0, file.Length);
                    await streamer.FlushAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private byte[] ResizeImage(string imagePath, int width, int height)
    {
        byte[] resizedImageBytes;

        using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(imagePath))
        {
            using (System.Drawing.Image resizedImage = new Bitmap(originalImage, new System.Drawing.Size(width, height)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Save(ms, originalImage.RawFormat);
                    resizedImageBytes = ms.ToArray();
                }
            }
        }

        return resizedImageBytes;
    }

}