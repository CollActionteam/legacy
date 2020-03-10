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
            Field<NonNullGraphType<IdGraphType>>(nameof(Tag.Id), resolve: x => x.Source.Id);
            Field(x => x.Name);
            AddNavigationListField(nameof(Tag.ProjectTags), c => c.Source.ProjectTags);
        }
    }
}
