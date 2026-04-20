using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherBartolome.Models
{
    public class SecuritySettings
    {
        public int Id { get; set; }
        public string ConfigPassword { get; set; } = "1234";
        public string TechPassword { get; set; } = "jillvalentine";
    }
}
