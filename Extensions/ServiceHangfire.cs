using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;

namespace Itsomax.Module.Core.Extensions
{
    public static class ServiceHangfire
    {
        public static IServiceCollection LoadHangfire (this IServiceCollection services,IConfigurationRoot configuration)
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