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
    }

    private void btnP1_Clicked(object sender, EventArgs e)
    {
        if (db.profiles != null && db.profiles.ContainsKey(btnP1.Text))
        {
            Navigation.PushAsync(new MacroMate(ip, port, btnP1.Text));
        }
    }
}