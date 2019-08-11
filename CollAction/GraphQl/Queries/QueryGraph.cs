using CollAction.Data;
using CollAction.Models;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using System.Linq;
using System.Security.Claims;

namespace CollAction.GraphQl.Queries
{
    public class QueryGraph : QueryGraphType<ApplicationDbContext>
    {
        public QueryGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
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
                c => c.DbContext.Users).AuthorizeWith(Constants.GraphQlAdminPolicy);

            AddSingleField(
                name: "user",
                resolve: c => c.DbContext.Users).AuthorizeWith(Constants.GraphQlAdminPolicy);

            AddSingleField(
                name: "currentUser",
                resolve: c =>
                {
                    var userContext = (UserContext)c.UserContext;
                    if (userContext.User != null)
                    {
                        string userId = userContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        return userContext.Context.Users.Where(u => u.Id == userId);
                    }
                    else
                    {
                        return userContext.Context.Users.Where(u => u.Id == null);
                    }
                });

            Field<DonationGraph>(
                "donation",
                resolve: c => new object());

            Field<MiscellaneousGraph>(
                "miscellaneous",
                resolve: c => new object());
        }
    }
} 