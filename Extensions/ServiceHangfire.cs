using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;

namespace Itsomax.Module.Core.Extensions
{
    public static class ServiceHangfire
    {
        public static IServiceCollection LoadHangFire (this IServiceCollection services,IConfiguration configuration)
        {
            var configDb = configuration.GetSection("UseConnection:DefaultConnection").Value;

            if(configDb=="Postgres")
            {
                services.AddHangfire(config =>
                    config.UsePostgreSqlStorage(configuration.GetConnectionString("Postgres"))
                );
            }
            if(configDb=="SqlServer")
            {
                services.AddHangfire(config =>
                config.UseSqlServerStorage(configuration.GetConnectionString("SqlServer")));
                
            }
            return services;
            
        }
    }
}