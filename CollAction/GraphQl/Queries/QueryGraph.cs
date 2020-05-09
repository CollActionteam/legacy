using CollAction.Data;
using CollAction.GraphQl.Mutations.Input;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Crowdactions;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
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
                name: nameof(ApplicationDbContext.Crowdactions),
                arguments: new QueryArgument[]
                {
                    new QueryArgument<SearchCrowdactionStatusInputGraph>() { Name = "status" },
                    new QueryArgument<CategoryGraph>() { Name = "category" }
                },
                resolve: c => 
                {
                    Category? category = c.GetArgument<Category?>("category");
                    SearchCrowdactionStatus? status = c.GetArgument<SearchCrowdactionStatus?>("status");

                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .SearchCrowdactions(category, status);
                });

            AddSingleField(
                name: nameof(Crowdaction),
                resolve: c => c.GetUserContext().ServiceProvider.GetRequiredService<ICrowdactionService>().SearchCrowdactions(null, null));

            FieldAsync<IntGraphType>(
                "crowdactionCount",
                arguments: new QueryArguments(
                    new QueryArgument<SearchCrowdactionStatusInputGraph>() { Name = "status" },
                    new QueryArgument<CategoryGraph>() { Name = "category" }),
                resolve: async c => 
                {
                    Category? category = c.GetArgument<Category?>("category");
                    SearchCrowdactionStatus? status = c.GetArgument<SearchCrowdactionStatus?>("status");

                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<ICrowdactionService>()
                                        .SearchCrowdactions(category, status)
                                        .CountAsync()
                                        .ConfigureAwait(false);
                });

            AddQueryField(
                nameof(ApplicationDbContext.Users),
                c => c.DbContext.Users,
                typeof(ApplicationUserGraph)).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            AddSingleField(
                name: "user",
                resolve: c => c.DbContext.Users,
                graphType: typeof(ApplicationUserGraph)).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<IntGraphType>(
                "userCount",
                resolve: async c => await c.GetUserContext().Context.Users.CountAsync().ConfigureAwait(false)).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

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
                graphType: typeof(ApplicationUserGraph),
                nullable: true);

            Field<MiscellaneousGraph>(
                "misc",
                resolve: c => new object());

            Field<StatisticsGraph>(
                "stats",
                resolve: c => new object());
        }
    }
} 