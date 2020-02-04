using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    // A restricted user type, to limit information exposure for users that have posted something
    public sealed class RestrictedApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public RestrictedApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.FirstName, true);
            Field(x => x.FullName, true);
            Field(x => x.LastName, true);
        }
    }
}