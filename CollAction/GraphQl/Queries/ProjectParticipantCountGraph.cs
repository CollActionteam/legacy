using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    public class ProjectParticipantCountGraph : EfObjectGraphType<ApplicationDbContext, ProjectParticipantCount>
    {
        public ProjectParticipantCountGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.Count);
        }
    }
}
