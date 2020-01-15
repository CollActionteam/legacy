using CollAction.Data;
using GraphQL.Authorization;
using System;
using System.Security.Claims;

namespace CollAction.GraphQl
{
    public sealed class UserContext : IProvideClaimsPrincipal
    {
        public UserContext(ClaimsPrincipal user, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            User = user;
            Context = context;
            ServiceProvider = serviceProvider;
        }

        public ClaimsPrincipal User { get; }

        public ApplicationDbContext Context { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}
