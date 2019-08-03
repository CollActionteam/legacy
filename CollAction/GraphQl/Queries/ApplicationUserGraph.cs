using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Donation;
using CollAction.Services.Newsletter;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CollAction.GraphQl.Queries
{
    public class ApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public ApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService, IDependencyResolver dependencyResolver, IServiceScopeFactory serviceScopeFactory) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Email);
            Field(x => x.FirstName);
            Field(x => x.FullName);
            Field(x => x.LastName);
            Field(x => x.RepresentsNumberParticipants);
            Field(x => x.UserName);
            Field(x => x.Activated);
            Field<BooleanGraphType>(
                "isAdmin", 
                resolve: c => ((UserContext)c.UserContext).User.IsInRole(Constants.AdminRole));
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
            AddNavigationListField(nameof(ApplicationUser.UserEvents), c => c.Source.UserEvents).AuthorizeWith(Constants.GraphQlAdminPolicy);
            AddNavigationListField(nameof(ApplicationUser.DonationEvents), c => c.Source.DonationEvents).AuthorizeWith(Constants.GraphQlAdminPolicy);
        }
    }
}
