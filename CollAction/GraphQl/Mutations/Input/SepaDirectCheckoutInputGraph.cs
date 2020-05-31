using CollAction.Services.Donation.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public sealed class SepaDirectCheckoutInputGraph : InputObjectGraphType<SepaDirectCheckout>
    {
        public SepaDirectCheckoutInputGraph()
        {
            Field(x => x.SourceId);
            Field(x => x.Name);
            Field(x => x.Email);
            Field(x => x.Amount);
        }
    }
}
