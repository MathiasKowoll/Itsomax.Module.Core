using Hangfire.Dashboard;

namespace Itsomax.Module.Core.Extensions
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole("Admin");
        }
    }
}