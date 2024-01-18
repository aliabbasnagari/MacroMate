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
        public string layout_dimensions { get; set; }
        public string layout_index { get; set; }
        public Dictionary<string, string> key_commands { get; set; }
    }
}