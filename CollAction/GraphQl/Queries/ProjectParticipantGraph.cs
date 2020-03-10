using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class ProjectParticipantGraph : EfObjectGraphType<ApplicationDbContext, ProjectParticipant>
    {
        public ProjectParticipantGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.SubscribedToProjectEmails);
            Field(x => x.UnsubscribeToken);
            Field(x => x.ParticipationDate);
            Field<NonNullGraphType<IdGraphType>>(nameof(ProjectParticipant.UserId), resolve: x => x.Source.UserId);
            Field<NonNullGraphType<IdGraphType>>(nameof(ProjectParticipant.ProjectId), resolve: x => x.Source.ProjectId);
            AddNavigationField(nameof(ProjectParticipant.Project), c => c.Source.Project);
            AddNavigationField(nameof(ProjectParticipant.User), c => c.Source.User, typeof(ApplicationUserGraph));
        }
    }
}
