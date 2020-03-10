using CollAction.Data;
using CollAction.Models;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    [GraphQLAuthorize(Policy = AuthorizationConstants.GraphQlAdminPolicy)]
    public sealed class DonationEventLogGraph : EfObjectGraphType<ApplicationDbContext, DonationEventLog>
    {
        public DonationEventLogGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>>(nameof(DonationEventLog.Id), resolve: x => x.Source.Id);
            Field(x => x.UserId, true);
            Field(x => x.EventData);

            AddNavigationField(nameof(DonationEventLog.User), c => c.Source.User, typeof(ApplicationUserGraph)).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
        }
    }
}
