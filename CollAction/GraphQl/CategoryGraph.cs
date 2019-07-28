using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl
{
    public class CategoryGraph : EfObjectGraphType<ApplicationDbContext, Category>
    {
        public CategoryGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Color);
            Field(x => x.ColorHex);
            Field(x => x.Name);
        }
    }
}
