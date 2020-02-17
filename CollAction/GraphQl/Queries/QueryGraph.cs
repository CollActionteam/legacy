using CollAction.Data;
using CollAction.GraphQl.Mutations.Input;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Projects;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Security.Claims;

namespace CollAction.GraphQl.Queries
{
    public sealed class QueryGraph : QueryGraphType<ApplicationDbContext>
    {
        public QueryGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            AddQueryField(
                name: nameof(ApplicationDbContext.Projects),
                arguments: new QueryArgument[]
                {
                    new QueryArgument<SearchProjectStatusInputGraph>() { Name = "status" },
                    new QueryArgument<CategoryGraph>() { Name = "category" }
                },
                resolve: c => 
                {
                    Category? category = c.GetArgument<Category?>("category");
                    SearchProjectStatus? status = c.GetArgument<SearchProjectStatus?>("status");

                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<IProjectService>()
                                  .SearchProjects(category, status);
                });

            AddSingleField(
                name: nameof(Project),
                resolve: c => c.DbContext.Projects);

            AddQueryField(
                nameof(ApplicationDbContext.Users),
                c => c.DbContext.Users,
                typeof(ApplicationUserGraph)).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            AddSingleField(
                name: "user",
                resolve: c => c.DbContext.Users,
                graphType: typeof(ApplicationUserGraph)).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

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
                        return userContext.Context.Users.Where(u => 0 == 1);
                    }
                },
                graphType: typeof(ApplicationUserGraph));

            Field<DonationGraph>(
                "donation",
                resolve: c => new object());

            Field<MiscellaneousGraph>(
                "miscellaneous",
                resolve: c => new object());

            Field<StatisticsGraph>(
                "statistics",
                resolve: c => new object());
        }
    }
} 