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
        public Dictionary<string, ProfileLayout>? profiles;
        private static DatabaseContext? _instance;

        private DatabaseContext()
        {

        }

        public static DatabaseContext getInstance()
        {
            if (_instance == null) _instance = new DatabaseContext();
            return _instance;
        }

        public ProfileLayout getLayout(string profile)
        {
            return profiles[profile];
        }

        public void LoadProfiles()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                profiles = JsonConvert.DeserializeObject<Dictionary<string, ProfileLayout>>(json);
            }
            else
            {
                profiles = new Dictionary<string, ProfileLayout>();
            }
        }

        public void UpdateProfiles()
        {
            string json = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Mappings saved to '{filePath}'.");
        }
    }
}
