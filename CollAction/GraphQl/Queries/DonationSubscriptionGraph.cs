using GraphQL.Types;
using Stripe;
using System;

namespace CollAction.GraphQl.Queries
{
    public sealed class DonationSubscriptionGraph : ObjectGraphType<Subscription>
    {
        public DonationSubscriptionGraph()
        {
            Field<NonNullGraphType<IdGraphType>, string>(nameof(Subscription.Id)).Resolve(x => x.Source.Id);
            Field<DateTimeOffsetGraphType, DateTime?>(nameof(Subscription.CanceledAt)).Resolve(x => x.Source.CanceledAt);
            Field<DateTimeOffsetGraphType, DateTime?>(nameof(Subscription.StartDate)).Resolve(x => x.Source.StartDate);
        }
    }
}
