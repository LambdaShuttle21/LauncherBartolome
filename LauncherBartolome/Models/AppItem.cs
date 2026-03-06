using System.Windows.Media;
using System.Text.Json.Serialization;
using System;

namespace LauncherBartolome.Models
{
    public class AppItem
    {
        public string Name { get; set; }
        public bool RequiresPassword { get; set; }

        public string ExecutablePath { get; set; }
        [JsonIgnore]//evita serializar el icono (tipo ImageSource) en el .json pero no se puede
        //porque ImageSource es un objeto complejo y no es serializable
        public ImageSource? Icon { get; set; }

        public bool isWeb => Uri.TryCreate(ExecutablePath, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    }
}

