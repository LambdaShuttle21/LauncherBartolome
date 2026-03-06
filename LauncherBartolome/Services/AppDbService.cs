using LauncherBartolome.Data;
using LauncherBartolome.Helpers;
using LauncherBartolome.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

namespace LauncherBartolome.Services
{
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
                var item = new AppItem//construye item(AppItem) a partir de var r con su nombre y ruta
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

                Apps.Add(item);//agrego a la observable list cada item

                Banners.Clear();
                foreach (var b in db.Banners.AsNoTracking().OrderBy(x => x.SortOrder).ToList())
                {
                    Banners.Add(b);
                }
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
    }
}
