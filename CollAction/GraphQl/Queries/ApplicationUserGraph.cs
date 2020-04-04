using CollAction.Data;
using CollAction.Helpers;
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
using System.Security.Claims;

namespace CollAction.GraphQl.Queries
{
    public sealed class ApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public ApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<IdGraphType>(nameof(ApplicationUser.Id), resolve: x => x.Source.Id);
            Field(x => x.Email);
            Field(x => x.FirstName, true);
            Field(x => x.FullName, true);
            Field(x => x.LastName, true);
            Field(x => x.RepresentsNumberParticipants);
            Field(x => x.UserName);
            Field(x => x.Activated);
            Field(x => x.RegistrationDate);
            FieldAsync<BooleanGraphType>(
                "isAdmin",
                resolve: async c =>
                {
                    ClaimsPrincipal principal = await c.GetUserContext().ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>().CreateUserPrincipalAsync(c.Source).ConfigureAwait(false);
                    return principal.IsInRole(AuthorizationConstants.AdminRole);
                });
            FieldAsync<BooleanGraphType>(
                "isSubscribedNewsletter", 
                resolve: async c =>
                {
                    return await c.GetUserContext().ServiceProvider.GetRequiredService<INewsletterService>().IsSubscribedAsync(c.Source.Email).ConfigureAwait(false);
                });
            FieldAsync<ListGraphType<DonationSubscriptionGraph>>(
                "donationSubscriptions",
                resolve: async c =>
                {
                    return await c.GetUserContext().ServiceProvider.GetRequiredService<IDonationService>().GetSubscriptionsFor(c.Source, c.CancellationToken).ConfigureAwait(false);
                });
            FieldAsync<ListGraphType<StringGraphType>>(
                "loginProviders",
                resolve: async c =>
                {
                    var userManager = c.GetUserContext().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var providers = await userManager.GetLoginsAsync(c.Source).ConfigureAwait(false);
                    return providers.Select(p => p.LoginProvider);
                });
            AddNavigationListField(nameof(ApplicationUser.Projects), c => c.Source.Projects);
            AddNavigationListField(nameof(ApplicationUser.Participates), c => c.Source.Participates);
            AddNavigationListField(nameof(ApplicationUser.UserEvents), c => c.Source.UserEvents).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
            AddNavigationListField(nameof(ApplicationUser.DonationEvents), c => c.Source.DonationEvents).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
        }
    }
}
