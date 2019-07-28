using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl
{
    public class ProjectParticipantGraph : EfObjectGraphType<ApplicationDbContext, ProjectParticipant>
    {
        public ProjectParticipantGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.SubscribedToProjectEmails);
            Field(x => x.UnsubscribeToken);
            AddNavigationField(nameof(ProjectParticipant.Project), c => c.Source.Project);
            AddNavigationField(nameof(ProjectParticipant.User), c => c.Source.User);
        }
    }
}
