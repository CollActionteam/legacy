using CollAction.Data;
using CollAction.Models;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    // A restricted user type, to limit information exposure for users that have posted something
    public sealed class RestrictedApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public RestrictedApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>, string>(nameof(ApplicationUser.Id)).Resolve(x => x.Source.Id);
            Field(x => x.FirstName, true);
            Field(x => x.FullName, true);
            Field(x => x.LastName, true);
            Field(x => x.Email).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
        }
    }
}