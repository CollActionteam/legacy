using GraphQL.Types;
using Stripe;

namespace CollAction.GraphQl.Queries
{
    public sealed class DonationSubscriptionGraph : ObjectGraphType<Subscription>
    {
        public DonationSubscriptionGraph()
        {
            Field(x => x.Id);
            Field(x => x.CanceledAt, true);
            Field(x => x.StartDate, true);
        }
    }
}
