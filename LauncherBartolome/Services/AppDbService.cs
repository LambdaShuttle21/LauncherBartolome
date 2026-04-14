using LauncherBartolome.Data;
using LauncherBartolome.Helpers;
using LauncherBartolome.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LauncherBartolome.Services
{
    public class BannerApiResponse
    {
        public List<BannerDto> Banners { get; set; } = new();
    }

    public class BannerDto
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string LinkUrl { get; set; } = "";
        public int SortOrder { get; set; }
    }
    public class AppDbService
    {
        public ObservableCollection<AppItem> Apps { get; } = new();
        public ObservableCollection<BannerEntity> Banners { get; } = new();
        public void Init()
        {
            using var db = new LauncherDbContext();
            db.Database.Migrate();

            if (!db.Banners.Any())
            {
                db.Banners.AddRange(
                    new BannerEntity
                    {
                        Title = "Soporte Tecnico",
                        Subtitle = "Accede a asistencia y mantenimiento para tus soluciones",
                        ImagePath = "pack://application:,,,/Assets/banner1.png",
                        LinkUrl = "https://www.bartolomeconsultores.com/technical-support/",
                        SortOrder = 1,
                    },
                    new BannerEntity
                    {
                        Title = "Productos y servicios",
                        Subtitle = "Descubre soluciones para TPV, gestion y comercio",
                        ImagePath = "pack://application:,,,/Assets/banner2.png",
                        LinkUrl = "https://www.bartolomeconsultores.com/",
                        SortOrder = 2
                    }
                    );

                db.SaveChanges();
            }
        }

        public void Load()
        {
            using var db = new LauncherDbContext();

            var rows = db.Apps
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .ToList();

            Apps.Clear();

            foreach (var r in rows)
            {
                var item = new AppItem
                {
                    Name = r.Name,
                    ExecutablePath = r.ExecutablePath
                };

                if (item.isWeb)
                {
                    item.Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/example.png"));
                }
                else
                {
                    IconHelper.TryLoadIcon(item);
                }

                Apps.Add(item);
            }
        }

        public void Save()
        {
            using var db = new LauncherDbContext();

            db.Apps.RemoveRange(db.Apps);
            db.SaveChanges();

            db.Apps.AddRange(Apps.Select(a => new AppEntity
            {
                Name = a.Name,
                ExecutablePath = a.ExecutablePath
            }));

            db.SaveChanges();
        }

        public async Task LoadBannersFromApiAsync()
        {
            using var client = new HttpClient();

            string json = await client.GetStringAsync("http://localhost:8080/launcher-data");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var response = JsonSerializer.Deserialize<BannerApiResponse>(json, options);

            Banners.Clear();

            if (response?.Banners != null)
            {
                foreach (var b in response.Banners.OrderBy(x => x.SortOrder))
                {
                    Banners.Add(new BannerEntity
                    {
                        Title = b.Title,
                        Subtitle = b.Subtitle,
                        ImagePath = b.ImagePath,
                        LinkUrl = b.LinkUrl,
                        SortOrder = b.SortOrder
                    });
                }
            }
        }
    }


}
