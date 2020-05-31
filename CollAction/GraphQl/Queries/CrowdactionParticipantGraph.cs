using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;
using System;

namespace CollAction.GraphQl.Queries
{
    public sealed class CrowdactionParticipantGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionParticipant>
    {
        public CrowdactionParticipantGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>, string>("id").Resolve(c => $"{c.Source.CrowdactionId}_{c.Source.UserId}");
            Field(x => x.SubscribedToCrowdactionEmails);
            Field(x => x.UnsubscribeToken);
            Field<NonNullGraphType<DateTimeOffsetGraphType>, DateTime>(nameof(CrowdactionParticipant.ParticipationDate)).Resolve(x => x.Source.ParticipationDate);
            Field<NonNullGraphType<IdGraphType>, string>(nameof(CrowdactionParticipant.UserId)).Resolve(x => x.Source.UserId);
            Field<NonNullGraphType<IdGraphType>, int>(nameof(CrowdactionParticipant.CrowdactionId)).Resolve(x => x.Source.CrowdactionId);
            AddNavigationField(nameof(CrowdactionParticipant.Crowdaction), c => c.Source.Crowdaction);
            AddNavigationField(nameof(CrowdactionParticipant.User), c => c.Source.User, typeof(ApplicationUserGraph));
        }
    }
}
