using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using MacroMate.Data;
using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json;

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
        Task.Run(StartServer);
        db.LoadProfiles();

        var tapGes = new TapGestureRecognizer();
        tapGes.Tapped += Profile_Clicked;
        foreach (var prof in db.profiles)
        {
            var get_icon = (File.Exists(prof.Value.icons["profile"]) ? prof.Value.icons["profile"] : "default_app_img.png");
            Image img = new Image();
            img.WidthRequest = 60;
            img.HeightRequest = 60;
            img.Source = get_icon;

            Label lbl = new Label();
            lbl.HorizontalOptions = LayoutOptions.Center;
            lbl.Text = $"{prof.Key}";
            lbl.FontSize = 18;

            VerticalStackLayout vsl = new VerticalStackLayout();
            vsl.WidthRequest = 200;
            vsl.HeightRequest = 200;
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

    private void Profile_Clicked(object sender, EventArgs e)
    {
        VerticalStackLayout btn = (VerticalStackLayout)sender;
        if (db.profiles != null && db.profiles.ContainsKey(btn.ClassId))
        {
            Navigation.PushAsync(new MacroMate(ip, port, btn.ClassId));
        }
    }

    private void btnCreate_Clicked(object sender, EventArgs e)
    {
    }

    /*
    private async Task StartServer()
    {
        try
        {
            string iconsDirectory = @"C:\Users\Ali Abbas\Documents\MacroMate\Icons";
            string[] images = Directory.GetFiles(iconsDirectory, "*.png");

            using (TcpListener server = new TcpListener(IPAddress.Parse(ip), port))
            {
                server.Start();

                using (TcpClient client = await server.AcceptTcpClientAsync())
                using (NetworkStream stream = client.GetStream())
                {
                    foreach (var imagePath in images)
                    {
                        byte[] imageData = await File.ReadAllBytesAsync(imagePath);
                        await stream.WriteAsync(imageData, 0, imageData.Length);
                        await Task.Delay(100);
                    }

                    // Signal the end of data transmission
                    byte[] endSignal = Encoding.UTF8.GetBytes("END");
                    await stream.WriteAsync(endSignal, 0, endSignal.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(() => DisplayAlert("1", ex.Message, "ok"));
        }
    }*/
    private async Task StartServer()
    {
        try
        {
            string iconsDirectory = @"C:\Users\Ali Abbas\Documents\MacroMate\Icons";
            string[] images = Directory.GetFiles(iconsDirectory, "*.png");

            using (TcpListener server = new TcpListener(IPAddress.Parse(ip), port))
            {
                server.Start();

                using (TcpClient client = await server.AcceptTcpClientAsync())
                using (NetworkStream stream = client.GetStream())
                {
                    for(int i = 0; i < 10; i++)
                    {
                        if (client.Connected)
                        {
                            byte[] imageData = await File.ReadAllBytesAsync(images[i]);
                            await WriteWithLengthPrefixAsync(stream, imageData);
                            await Task.Delay(100);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Dispatcher.Dispatch(() => DisplayAlert("Here 1", ex.Message, "ok"));
        }
    }

    private async Task WriteWithLengthPrefixAsync(NetworkStream stream, byte[] data)
    {
        byte[] len = Encoding.UTF8.GetBytes($"{data.Length}");
        await stream.WriteAsync(len, 0, len.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }

}