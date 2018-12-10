using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace Itsomax.Module.Core.Extensions
{
    public static class ApplicationBuilderHangFire
    {
        public static IApplicationBuilder UseCustomizedHangFire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            return app;
        }
    }
}