using CollAction.Data;
using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Festival;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CollAction.GraphQl.Queries
{
    public class QueryGraph : QueryGraphType<ApplicationDbContext>
    {
        public QueryGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService, IServiceScopeFactory serviceScopeFactory, IFestivalService festivalService, IOptions<DisqusOptions> disqusOptions) : base(entityFrameworkGraphQlService)
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

            FieldAsync<ApplicationUserGraph>(
                name: "currentUser",
                resolve: async c =>
                {
                    var user = ((UserContext)c.UserContext).User;
                    if (user != null)
                    {
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                            return await userManager.GetUserAsync(user);
                        }
                    }
                    else
                    {
                        return null;
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