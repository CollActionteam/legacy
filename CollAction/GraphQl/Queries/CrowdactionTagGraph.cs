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
            Field<NonNullGraphType<IdGraphType>, int>(nameof(CrowdactionTag.CrowdactionId)).Resolve(x => x.Source.CrowdactionId);
            Field<NonNullGraphType<IdGraphType>, int>(nameof(CrowdactionTag.TagId)).Resolve(x => x.Source.TagId);
            AddNavigationField(nameof(CrowdactionTag.Crowdaction), c => c.Source.Crowdaction);
            AddNavigationField(nameof(CrowdactionTag.Tag), c => c.Source.Tag);
        }
    }
}
