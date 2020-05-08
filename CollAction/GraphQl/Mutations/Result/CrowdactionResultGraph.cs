using CollAction.Services.Crowdactions.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Result
{
    public sealed class CrowdactionResultGraph : ObjectGraphType<CrowdactionResult>
    {
        public CrowdactionResultGraph()
        {
            Field(x => x.Crowdaction, true);
            Field(x => x.Succeeded);
            Field(x => x.Errors);
        }
    }
}
