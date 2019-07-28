using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl
{
    public class TagGraph : EfObjectGraphType<ApplicationDbContext, Tag>
    {
        public TagGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Name);
            AddNavigationField(nameof(Tag.ProjectTags), c => c.Source.ProjectTags);
        }
    }
}
