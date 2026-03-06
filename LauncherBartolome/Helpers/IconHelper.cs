using LauncherBartolome.Models;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LauncherBartolome.Helpers
{
    public static class IconHelper
    {
        public static void TryLoadIcon(AppItem app)
        {
            try
            {
                if (app == null) return;

                var exePath = ResolveExecutablePath(app.ExecutablePath);
                if (exePath == null) return;

                using var icon = Icon.ExtractAssociatedIcon(exePath);
                if (icon == null) return;

                app.Icon = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(32, 32));
            }
            catch
            {
                // ignorar
            }
        }

        private static string? ResolveExecutablePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            if (File.Exists(path))
                return path;

            // para "notepad.exe", "calc.exe", etc (PATH)
            var envPath = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrWhiteSpace(envPath)) return null;

            foreach (var dir in envPath.Split(';'))
            {
                try
                {
                    var full = System.IO.Path.Combine(dir.Trim(), path);
                    if (File.Exists(full))
                        return full;
                }
                catch { }
            }

            return null;
        }
    }
}
