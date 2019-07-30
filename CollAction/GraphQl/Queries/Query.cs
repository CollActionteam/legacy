using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    public class Query :
        QueryGraphType<ApplicationDbContext>
    {
        public Query(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            AddQueryField(
                nameof(ApplicationDbContext.Projects),
                c => c.DbContext.Projects);

            AddSingleField(
                name: nameof(Project),
                resolve: c => c.DbContext.Projects);

            AddQueryField(
                nameof(ApplicationDbContext.Categories),
                c => c.DbContext.Categories);

            AddSingleField(
                name: nameof(Category),
                resolve: c => c.DbContext.Categories);

            AddQueryField(
                nameof(ApplicationDbContext.Users),
                c => c.DbContext.Users);

            AddSingleField(
                name: "User",
                resolve: c => c.DbContext.Users);
        }
    }
} 