using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class CrowdactionTagGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionTag>
    {
        public CrowdactionTagGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>>(nameof(CrowdactionTag.CrowdactionId), resolve: x => x.Source.CrowdactionId);
            Field<NonNullGraphType<IdGraphType>>(nameof(CrowdactionTag.TagId), resolve: x => x.Source.TagId);
            AddNavigationField(nameof(CrowdactionTag.Crowdaction), c => c.Source.Crowdaction);
            AddNavigationField(nameof(CrowdactionTag.Tag), c => c.Source.Tag);
        }
    }
}
