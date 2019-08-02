using CollAction.Data;
using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Donation;
using CollAction.Services.Festival;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CollAction.GraphQl.Queries
{
    public class Query : QueryGraphType<ApplicationDbContext>
    {
        public Query(IEfGraphQLService<ApplicationDbContext> efGraphQlService, IServiceScopeFactory serviceScopeFactory, IFestivalService festivalService, IOptions<DisqusOptions> disqusOptions) : base(efGraphQlService)
        {
            AddQueryField(
                nameof(ApplicationDbContext.Projects),
                c => c.DbContext.Projects);

            AddQueryField(
                nameof(ApplicationDbContext.Categories),
                c => c.DbContext.Categories);

            AddQueryField(
                nameof(ApplicationDbContext.Users),
                c => c.DbContext.Users);

            FieldAsync<ApplicationUserGraph>(
                name: "currentUser",
                resolve: async c =>
                {
                    var user = ((UserContext)c.UserContext).User;
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        return await userManager.GetUserAsync(user);
                    }
                });

            Field<BooleanGraphType>(
                nameof(IFestivalService.CallToActionsVisible), 
                resolve: c => festivalService.CallToActionsVisible);

            Field<StringGraphType>(
                nameof(DisqusOptions.DisqusSite),
                resolve: c => disqusOptions.Value.DisqusSite);

            FieldAsync<BooleanGraphType>(
                "hasPaymentSucceeded",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType>() { Name = "source" },
                    new QueryArgument<StringGraphType>() { Name = "clientSecret" }),
                resolve: async c =>
                {
                    string source = c.GetArgument<string>("source");
                    string clientSecret = c.GetArgument<string>("clientSecret");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IDonationService>().HasIdealPaymentSucceeded(source, clientSecret);
                    }
                });
        }
    }
} 