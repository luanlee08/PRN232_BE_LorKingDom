using Hangfire.Dashboard;

namespace PRN232_LorKingDom.Middleware
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Allow access in development
            // In production, you should implement proper authentication/authorization
            var httpContext = context.GetHttpContext();

            // For now, allow all access (you should add proper authentication)
            // TODO: Add authentication check for admin users
            return true;
        }
    }
}
