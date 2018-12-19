using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace Itsomax.Module.Core.Extensions
{
    public static class ApplicationBuilderHangFire
    {
        public static IApplicationBuilder UseCustomizedHangFire(this IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() },
            });
            return app;
        }
    }
}