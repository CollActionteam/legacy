using CollAction.Data;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Donation;
using CollAction.Services.Newsletter;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CollAction.GraphQl.Queries
{
    public sealed class ApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public ApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>, string>(nameof(ApplicationUser.Id)).Resolve(x => x.Source.Id);
            Field(x => x.Email);
            Field(x => x.FirstName, true);
            Field(x => x.FullName, true);
            Field(x => x.LastName, true);
            Field(x => x.RepresentsNumberParticipants);
            Field(x => x.UserName);
            Field(x => x.Activated);
            Field<NonNullGraphType<DateTimeOffsetGraphType>, DateTime>(nameof(ApplicationUser.RegistrationDate)).Resolve(x => x.Source.RegistrationDate);
            FieldAsync<NonNullGraphType<BooleanGraphType>, bool>(
                "isAdmin",
                resolve: async c =>
                {
                    ClaimsPrincipal principal = await c.GetUserContext().ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>().CreateUserPrincipalAsync(c.Source).ConfigureAwait(false);
                    return principal.IsInRole(AuthorizationConstants.AdminRole);
                });
            FieldAsync<NonNullGraphType<BooleanGraphType>, bool>(
                "isSubscribedNewsletter",
                resolve: c =>
                {
                    return c.GetUserContext()
                            .ServiceProvider
                            .GetRequiredService<INewsletterService>()
                            .IsSubscribedAsync(c.Source.Email);
                });
            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<DonationSubscriptionGraph>>>, IEnumerable<Subscription>>(
                "donationSubscriptions",
                resolve: c =>
                {
                    return c.GetUserContext()
                            .ServiceProvider
                            .GetRequiredService<IDonationService>()
                            .GetSubscriptionsFor(c.Source, c.CancellationToken);
                });
            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>, IEnumerable<string>>(
                "loginProviders",
                resolve: async c =>
                {
                    var userManager = c.GetUserContext().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var providers = await userManager.GetLoginsAsync(c.Source).ConfigureAwait(false);
                    return providers.Select(c => c.LoginProvider);
                });
            AddNavigationListField(nameof(ApplicationUser.Crowdactions), c => c.Source.Crowdactions);
            AddNavigationListField(nameof(ApplicationUser.Participates), c => c.Source.Participates);
            AddNavigationListField(nameof(ApplicationUser.UserEvents), c => c.Source.UserEvents).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
            AddNavigationListField(nameof(ApplicationUser.DonationEvents), c => c.Source.DonationEvents).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
        }
    }
}
