using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace CollAction.Helpers
{
    public sealed class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
            => context.GetHttpContext().User?.IsInRole(AuthorizationConstants.AdminRole) ?? false;
    }
}
