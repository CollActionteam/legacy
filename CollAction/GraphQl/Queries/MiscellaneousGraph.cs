using CollAction.Models;
using CollAction.Services;
using CollAction.Services.Festival;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;

namespace CollAction.GraphQl.Queries
{
    public class MiscellaneousGraph : ObjectGraphType
    {
        public MiscellaneousGraph(IServiceScopeFactory serviceScopeFactory, IFestivalService festivalService, IOptions<DisqusOptions> disqusOptions)
        {
            Field<BooleanGraphType>(
                nameof(IFestivalService.FestivalCallToActionVisible), 
                resolve: c => festivalService.FestivalCallToActionVisible);

            Field<StringGraphType>(
                nameof(DisqusOptions.DisqusSite),
                resolve: c => disqusOptions.Value.DisqusSite);

            FieldAsync<ListGraphType<StringGraphType>>(
                "externalLoginProviders",
                resolve: async c =>
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var externalSchemes = 
                            await scope.ServiceProvider
                                       .GetRequiredService<SignInManager<ApplicationUser>>()
                                       .GetExternalAuthenticationSchemesAsync();
                        return externalSchemes.Select(s => s.Name);
                    }
                });
        }
    }
}
