using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class CrowdactionParticipantCountGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionParticipantCount>
    {
        public CrowdactionParticipantCountGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>, int>(nameof(CrowdactionParticipantCount.CrowdactionId)).Resolve(x => x.Source.CrowdactionId);
            Field(x => x.Count);
        }
    }
}
