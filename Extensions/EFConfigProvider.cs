using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Itsomax.Module.Core.Extensions
{
    public class EfConfigProvider : ConfigurationProvider
    {
        private Action<DbContextOptionsBuilder> OptionsAction { get; }

        public EfConfigProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<EfConfigurationDbContext>();
            OptionsAction(builder);

            using (var dbContext = new EfConfigurationDbContext(builder.Options))
            {
                Data = dbContext.AppSettings.ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}