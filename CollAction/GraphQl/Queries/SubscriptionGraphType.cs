using GraphQL.Types;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.GraphQl.Queries
{
    public class SubscriptionGraphType : ObjectGraphType<Subscription>
    {
        public SubscriptionGraphType()
        {
            Field(x => x.Id);
            Field(x => x.CanceledAt, true);
            Field(x => x.StartDate, true);
        }
    }
}
