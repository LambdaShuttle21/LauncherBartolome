using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherBartolome.Data
{
    public class BannerEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string LinkUrl { get; set; } = "";
        public int SortOrder { get; set; } = 0;
    }
}
