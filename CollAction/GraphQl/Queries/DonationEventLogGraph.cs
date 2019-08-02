using CollAction.Data;
using CollAction.Models;
using GraphQL.Authorization;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    [GraphQLAuthorize(Policy = Constants.GraphQlAdminPolicy)]
    public class DonationEventLogGraph : EfObjectGraphType<ApplicationDbContext, DonationEventLog>
    {
        public DonationEventLogGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.User);
            Field(x => x.UserId);
            Field(x => x.EventData);
        }
    }
}
