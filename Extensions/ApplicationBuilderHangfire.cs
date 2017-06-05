using Microsoft.AspNetCore.Builder;
using Hangfire;

namespace Itsomax.Module.Core.Extensions
{
    public static class ApplicationBuilderHangfire
    {
        public static IApplicationBuilder UseCustomizedHanfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            return app;
        }
    }
}