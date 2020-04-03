using CollAction.Services.Donation.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public class IDealCheckoutInputGraph : InputObjectGraphType<IDealCheckout>
    {
        public IDealCheckoutInputGraph()
        {
            Field(x => x.SourceId);
            Field(x => x.Name);
            Field(x => x.Email);
        }
    }
}
