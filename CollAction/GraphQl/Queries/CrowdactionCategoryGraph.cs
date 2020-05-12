using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class CrowdactionCategoryGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionCategory>
    {
        public CrowdactionCategoryGraph(IEfGraphQLService<ApplicationDbContext> graphService) : base(graphService)
        {
            Field(x => x.Category);
            Field<NonNullGraphType<IdGraphType>>(nameof(CrowdactionCategory.CrowdactionId), resolve: x => x.Source.CrowdactionId);
            AddNavigationField(nameof(CrowdactionCategory.Crowdaction), x => x.Source.Crowdaction);
        }
    }
}
