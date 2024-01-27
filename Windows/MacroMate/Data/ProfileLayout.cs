using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MacroMate.Data
{

    public class ProfileLayout
    {
        public int rows { get; set; }
        public int columns { get; set; }
        public string profile_icon { get; set; }
        public Dictionary<string, string> key_commands { get; set; }
        public Dictionary<string, string> icons { get; set; }
    }
}