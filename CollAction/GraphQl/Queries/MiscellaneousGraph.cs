using CollAction.Models;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CollAction.GraphQl.Queries
{
    public sealed class MiscellaneousGraph : ObjectGraphType
    {
        public MiscellaneousGraph(IServiceScopeFactory serviceScopeFactory)
        {
            FieldAsync<ListGraphType<StringGraphType>>(
                "externalLoginProviders",
                resolve: async c =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var externalSchemes =
                        await scope.ServiceProvider
                            .GetRequiredService<SignInManager<ApplicationUser>>()
                            .GetExternalAuthenticationSchemesAsync();
                    return externalSchemes.Select(s => s.Name);
                });
        }
    }
}
