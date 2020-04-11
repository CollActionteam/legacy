using CollAction.Models;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Extensions.Options;
using CollAction.Services.Newsletter;
using CollAction.Helpers;
using CollAction.Services;
using CollAction.Services.Donation;

namespace CollAction.GraphQl.Queries
{
    public sealed class MiscellaneousGraph : ObjectGraphType
    {
        public MiscellaneousGraph(IOptions<NewsletterServiceOptions> newsletterServiceOptions, IOptions<DisqusOptions> disqusOptions, IOptions<StripePublicOptions> stripePublicOptions)
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
                nameof(disqusOptions.Value.DisqusSiteId),
                resolve: c => disqusOptions.Value.DisqusSiteId);

            Field<NonNullGraphType<StringGraphType>>(
                nameof(stripePublicOptions.Value.StripePublicKey),
                resolve: c => stripePublicOptions.Value.StripePublicKey);

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
