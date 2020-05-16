﻿using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Donation;
using CollAction.Services.Newsletter;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;

namespace CollAction.GraphQl.Queries
{
    public sealed class MiscellaneousGraph : ObjectGraphType
    {
        public MiscellaneousGraph(IOptions<NewsletterServiceOptions> newsletterServiceOptions, IOptions<StripePublicOptions> stripePublicOptions, IOptions<AnalyticsOptions> analyticsOptions)
        {
            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>(
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

            Field<NonNullGraphType<StringGraphType>>(
                nameof(newsletterServiceOptions.Value.MailChimpNewsletterListId),
                resolve: c => newsletterServiceOptions.Value.MailChimpNewsletterListId);

            Field<NonNullGraphType<StringGraphType>>(
                nameof(newsletterServiceOptions.Value.MailChimpServer),
                resolve: c => newsletterServiceOptions.Value.MailChimpServer);

            Field<NonNullGraphType<StringGraphType>>(
                nameof(newsletterServiceOptions.Value.MailChimpUserId),
                resolve: c => newsletterServiceOptions.Value.MailChimpUserId);

            Field<NonNullGraphType<StringGraphType>>(
                nameof(newsletterServiceOptions.Value.MailChimpAccount),
                resolve: c => newsletterServiceOptions.Value.MailChimpAccount);

            Field<NonNullGraphType<StringGraphType>>(
                nameof(stripePublicOptions.Value.StripePublicKey),
                resolve: c => stripePublicOptions.Value.StripePublicKey);

            Field<NonNullGraphType<StringGraphType>>(
                nameof(analyticsOptions.Value.GoogleAnalyticsID),
                resolve: c => analyticsOptions.Value.GoogleAnalyticsID);

            Field<NonNullGraphType<StringGraphType>>(
                nameof(analyticsOptions.Value.FacebookPixelID),
                resolve: c => analyticsOptions.Value.FacebookPixelID);

            FieldAsync<BooleanGraphType>(
                "hasIDealPaymentSucceeded",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "source" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "clientSecret" }),
                resolve: async c =>
                {
                    string source = c.GetArgument<string>("source");
                    string clientSecret = c.GetArgument<string>("clientSecret");
                    return await c.GetUserContext().ServiceProvider.GetRequiredService<IDonationService>().HasIDealPaymentSucceeded(source, clientSecret, c.CancellationToken).ConfigureAwait(false);
                });
        }
    }
}
