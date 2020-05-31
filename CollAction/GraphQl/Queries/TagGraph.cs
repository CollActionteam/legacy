using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class TagGraph : EfObjectGraphType<ApplicationDbContext, Tag>
    {
        public TagGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>, int>(nameof(Tag.Id)).Resolve(x => x.Source.Id);
            Field(x => x.Name);
            AddNavigationListField(nameof(Tag.CrowdactionTags), c => c.Source.CrowdactionTags);
        }
    }
}
