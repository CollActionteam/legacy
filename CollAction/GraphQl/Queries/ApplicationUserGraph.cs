using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Donation;
using CollAction.Services.Newsletter;
using GraphQL;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CollAction.GraphQl.Queries
{
    public class ApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public ApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService, IDependencyResolver dependencyResolver, IServiceScopeFactory serviceScopeFactory): base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Email);
            Field(x => x.FirstName);
            Field(x => x.FullName);
            Field(x => x.LastName);
            Field(x => x.RepresentsNumberParticipants);
            Field(x => x.UserName);
            Field(x => x.Activated);
            FieldAsync<BooleanGraphType>(
                "isSubscribedNewsletter", 
                resolve: async c =>
                {
                    return await dependencyResolver.Resolve<INewsletterService>().IsSubscribedAsync(c.Source.Email);
                });
            FieldAsync<ListGraphType<DonationSubscriptionGraph>>(
                "donationSubscriptions",
                resolve: async c =>
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IDonationService>().GetSubscriptionsFor(c.Source);
                    }
                });
            FieldAsync<ListGraphType<StringGraphType>>(
                "roles", 
                resolve: async c =>
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        return await userManager.GetRolesAsync(c.Source);
                    }
                });
            FieldAsync<ListGraphType<StringGraphType>>(
                "loginProviders",
                resolve: async c =>
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        var providers = await userManager.GetLoginsAsync(c.Source);
                        return providers.Select(p => p.LoginProvider);
                    }
                });
            AddNavigationListField(nameof(ApplicationUser.Projects), c => c.Source.Projects);
            AddNavigationListField(nameof(ApplicationUser.Participates), c => c.Source.Participates);
            AddNavigationListField(nameof(ApplicationUser.UserEvents), c => c.Source.UserEvents);
            AddNavigationListField(nameof(ApplicationUser.DonationEvents), c => c.Source.DonationEvents);
        }
    }
}
