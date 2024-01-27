using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroMate.Data
{
    public class DatabaseContext
    {
        private string filePath = "C:/Users/Ali Abbas/Documents/MacroMate/profiles.json";
        public Dictionary<string, ProfileLayout> profiles;
        private static DatabaseContext? _instance;

        private DatabaseContext()
        {
            profiles = LoadProfiles();
        }

        public static DatabaseContext getInstance()
        {
            if (_instance == null) _instance = new DatabaseContext();
            return _instance;
        }

        private Dictionary<string, ProfileLayout> LoadProfiles()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Dictionary<string, ProfileLayout>>(json) ?? new Dictionary<string, ProfileLayout>();
            }
            else
            {
                return new Dictionary<string, ProfileLayout>();
            }
        }

        public void UpdateProfiles()
        {
            string json = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Mappings saved to '{filePath}'.");
        }

        public void Refresh()
        {
            profiles = LoadProfiles();
        }
    }
}
