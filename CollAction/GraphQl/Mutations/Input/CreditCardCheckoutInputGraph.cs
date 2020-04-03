using CollAction.Services.Donation.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public class CreditCardCheckoutInputGraph : InputObjectGraphType<CreditCardCheckout>
    {
        public CreditCardCheckoutInputGraph()
        {
            Field(x => x.Currency);
            Field(x => x.Name);
            Field(x => x.Email);
            Field(x => x.Amount);
            Field(x => x.Recurring);
            Field(x => x.SuccessUrl);
            Field(x => x.CancelUrl);
        }
    }
}
