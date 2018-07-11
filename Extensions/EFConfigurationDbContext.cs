using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.Core.Extensions
{
    public class EfConfigurationDbContext : DbContext
    {
        public EfConfigurationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}