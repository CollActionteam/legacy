using CollAction.Data;
using CollAction.Models;
using GraphQL.Authorization;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    [GraphQLAuthorize(Policy = AuthorizationConstants.GraphQlAdminPolicy)]
    public sealed class DonationEventLogGraph : EfObjectGraphType<ApplicationDbContext, DonationEventLog>
    {
        public DonationEventLogGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.UserId, true);
            Field(x => x.EventData);

            AddNavigationField(nameof(DonationEventLog.User), c => c.Source.User, typeof(ApplicationUserGraph)).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
        }
    }
}
