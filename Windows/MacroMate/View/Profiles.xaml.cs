using System.Xml;
using Newtonsoft.Json;

namespace MacroMate.View;

public partial class Profiles : ContentPage
{
    private string ip;
    private int port;
    private Dictionary<string, Dictionary<string, string>> profiles;
    public Profiles(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        InitializeComponent();
        profiles = LoadMappingsFromFile("C:/Users/Ali Abbas/Documents/MacroMate/profiles.json");
    }

    static void SaveMappingsToFile(string filePath, Dictionary<string, Dictionary<string, string>> allMappings)
    {
        string json = JsonConvert.SerializeObject(allMappings, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
        Console.WriteLine($"Mappings saved to '{filePath}'.");
    }

    private Dictionary<string, Dictionary<string, string>> LoadMappingsFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Dictionary<string, Dictionary<string, string>>? dict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            return (dict != null) ? dict : new Dictionary<string, Dictionary<string, string>>();
        }
        else
        {
            return new Dictionary<string, Dictionary<string, string>>();
        }
    }

    private void btnP1_Clicked(object sender, EventArgs e)
    {
        if (profiles.ContainsKey(btnP1.Text))
        {
            Navigation.PushAsync(new MacroMate(ip, port, btnP1.Text));
        }
    }
}