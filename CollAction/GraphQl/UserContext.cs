using CollAction.Data;
using GraphQL.Authorization;
using System;
using System.Security.Claims;

namespace CollAction.GraphQl
{
    public class UserContext : IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }

        public ApplicationDbContext Context { get; set; }

        public IServiceProvider ServiceProvider { get; set; }
    }
}
