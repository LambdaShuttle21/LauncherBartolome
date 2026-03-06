using Microsoft.EntityFrameworkCore;

namespace LauncherBartolome.Data
{
    public class LauncherDbContext : DbContext
    {
        public DbSet<AppEntity> Apps => Set<AppEntity>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=launcher.db");

        public DbSet<BannerEntity> Banners => Set<BannerEntity>();
    }
}