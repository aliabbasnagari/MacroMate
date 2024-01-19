using System.Xml;
using MacroMate.Data;
using Newtonsoft.Json;

namespace MacroMate.View;

public partial class Profiles : ContentPage
{
    private string ip;
    private int port;
    private DatabaseContext db = DatabaseContext.getInstance();
    public Profiles(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        InitializeComponent();
        db.LoadProfiles();

        // DisplayAlert("1", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MacroMate") ,"ok");

        var tapGes = new TapGestureRecognizer();
        tapGes.Tapped += Profile_Clicked;
        foreach (var prof in db.profiles)
        {
            Image img = new Image();
            img.WidthRequest = 60;
            img.HeightRequest = 60;
            img.Source = prof.Value.icons["profile"];

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
}