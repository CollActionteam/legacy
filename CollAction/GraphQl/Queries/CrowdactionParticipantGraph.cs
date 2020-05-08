using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class CrowdactionParticipantGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionParticipant>
    {
        public CrowdactionParticipantGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<IdGraphType>("id", resolve: c => $"{c.Source.CrowdactionId}_{c.Source.UserId}");
            Field(x => x.SubscribedToCrowdactionEmails);
            Field(x => x.UnsubscribeToken);
            Field<NonNullGraphType<DateTimeOffsetGraphType>>(nameof(CrowdactionParticipant.ParticipationDate), resolve: x => x.Source.ParticipationDate);
            Field<NonNullGraphType<IdGraphType>>(nameof(CrowdactionParticipant.UserId), resolve: x => x.Source.UserId);
            Field<NonNullGraphType<IdGraphType>>(nameof(CrowdactionParticipant.CrowdactionId), resolve: x => x.Source.CrowdactionId);
            AddNavigationField(nameof(CrowdactionParticipant.Crowdaction), c => c.Source.Crowdaction);
            AddNavigationField(nameof(CrowdactionParticipant.User), c => c.Source.User, typeof(ApplicationUserGraph));
        }
    }
}
