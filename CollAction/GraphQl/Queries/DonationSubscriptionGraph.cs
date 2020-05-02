using GraphQL.Types;
using Stripe;

namespace CollAction.GraphQl.Queries
{
    public sealed class DonationSubscriptionGraph : ObjectGraphType<Subscription>
    {
        public DonationSubscriptionGraph()
        {
            Field<NonNullGraphType<IdGraphType>>(nameof(Subscription.Id), resolve: x => x.Source.Id);
            Field<DateTimeOffsetGraphType>(nameof(Subscription.CanceledAt), resolve: x => x.Source.CanceledAt);
            Field<DateTimeOffsetGraphType>(nameof(Subscription.StartDate), resolve: x => x.Source.StartDate);
        }
    }
}
