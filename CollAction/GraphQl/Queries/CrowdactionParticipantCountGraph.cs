using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    public sealed class CrowdactionParticipantCountGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionParticipantCount>
    {
        public CrowdactionParticipantCountGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.Count);
        }
    }
}
