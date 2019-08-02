using CollAction.Data;
using CollAction.Models;
using GraphQL.Authorization;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    [GraphQLAuthorize(Policy = Constants.GraphQlAdminPolicy)]
    public class UserEventGraph : EfObjectGraphType<ApplicationDbContext, UserEvent>
    {
        public UserEventGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.EventData);
            Field(x => x.EventLoggedAt);
            Field(x => x.User);
            Field(x => x.UserId);
        }
    }
}
