using CollAction.Services;
using CollAction.Services.Festival;
using GraphQL.Types;
using Microsoft.Extensions.Options;

namespace CollAction.GraphQl.Queries
{
    public class MiscellaneousGraph : ObjectGraphType
    {
        public MiscellaneousGraph(IFestivalService festivalService, IOptions<DisqusOptions> disqusOptions)
        {
            Field<BooleanGraphType>(
                nameof(IFestivalService.FestivalCallToActionVisible), 
                resolve: c => festivalService.FestivalCallToActionVisible);

            Field<StringGraphType>(
                nameof(DisqusOptions.DisqusSite),
                resolve: c => disqusOptions.Value.DisqusSite);
        }
    }
}
