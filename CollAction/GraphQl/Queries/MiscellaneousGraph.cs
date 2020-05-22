using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Donation;
using CollAction.Services.Newsletter;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace CollAction.GraphQl.Queries
{
    public sealed class MiscellaneousGraph : ObjectGraphType
    {
        public MiscellaneousGraph(IOptions<NewsletterServiceOptions> newsletterServiceOptions, IOptions<DisqusOptions> disqusOptions, IOptions<StripePublicOptions> stripePublicOptions, IOptions<AnalyticsOptions> analyticsOptions)
        {
            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>, IEnumerable<string>>(
                "externalLoginProviders",
                resolve: async c =>
                {
                    var externalSchemes =
                        await c.GetUserContext()
                               .ServiceProvider
                               .GetRequiredService<SignInManager<ApplicationUser>>()
                               .GetExternalAuthenticationSchemesAsync()
                               .ConfigureAwait(false);
                    return externalSchemes.Select(s => s.Name);
                });

            Field<NonNullGraphType<StringGraphType>, string>(nameof(newsletterServiceOptions.Value.MailChimpNewsletterListId))
                .Resolve(c => newsletterServiceOptions.Value.MailChimpNewsletterListId);

            Field<NonNullGraphType<StringGraphType>, string>(nameof(newsletterServiceOptions.Value.MailChimpServer))
                .Resolve(c => newsletterServiceOptions.Value.MailChimpServer);

            Field<NonNullGraphType<StringGraphType>, string>(nameof(newsletterServiceOptions.Value.MailChimpUserId))
                .Resolve(c => newsletterServiceOptions.Value.MailChimpUserId);

            Field<NonNullGraphType<StringGraphType>, string>(nameof(newsletterServiceOptions.Value.MailChimpAccount))
                .Resolve(c => newsletterServiceOptions.Value.MailChimpAccount);

            Field<NonNullGraphType<StringGraphType>, string>(nameof(disqusOptions.Value.DisqusSiteId))
                .Resolve(c => disqusOptions.Value.DisqusSiteId);

            Field<NonNullGraphType<StringGraphType>, string>(nameof(stripePublicOptions.Value.StripePublicKey))
                .Resolve(c => stripePublicOptions.Value.StripePublicKey);

            Field<NonNullGraphType<StringGraphType>, string>(nameof(analyticsOptions.Value.GoogleAnalyticsID))
                .Resolve(c => analyticsOptions.Value.GoogleAnalyticsID);

            Field<NonNullGraphType<StringGraphType>, string>(nameof(analyticsOptions.Value.FacebookPixelID))
                .Resolve(c => analyticsOptions.Value.FacebookPixelID);

            FieldAsync<BooleanGraphType, bool>(
                "hasIDealPaymentSucceeded",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "source" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "clientSecret" }),
                resolve: c =>
                {
                    string source = c.GetArgument<string>("source");
                    string clientSecret = c.GetArgument<string>("clientSecret");
                    return c.GetUserContext().ServiceProvider.GetRequiredService<IDonationService>().HasIDealPaymentSucceeded(source, clientSecret, c.CancellationToken);
                });
        }
    }
}
