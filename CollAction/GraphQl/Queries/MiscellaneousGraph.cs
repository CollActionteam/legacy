using CollAction.Models;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Extensions.Options;
using CollAction.Services.Newsletter;
using CollAction.Helpers;

namespace CollAction.GraphQl.Queries
{
    public sealed class MiscellaneousGraph : ObjectGraphType
    {
        public MiscellaneousGraph(IOptions<NewsletterServiceOptions> NewsletterServiceOptions)
        {
            FieldAsync<ListGraphType<StringGraphType>>(
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

            Field<StringGraphType>(
                "mailChimpNewsletterListId",
                resolve: c =>
                {
                    return NewsletterServiceOptions.Value.MailChimpNewsletterListId;
                });
        }
    }
}
