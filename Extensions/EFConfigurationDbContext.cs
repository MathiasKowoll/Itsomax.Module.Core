using Microsoft.EntityFrameworkCore;
using Itsomax.Module.Core.Models;

namespace Itsomax.Module.Core.Extensions
{
    public class EFConfigurationDbContext : DbContext
    {
        public EFConfigurationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppSetting> AppSettings { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSetting>().ToTable("Core_AppSetting");
        }

    }
}