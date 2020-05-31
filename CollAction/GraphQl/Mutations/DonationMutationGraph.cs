using CollAction.GraphQl.Mutations.Input;
using CollAction.Helpers;
using CollAction.Services.Donation;
using CollAction.Services.Donation.Models;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Mutations
{
    public sealed class DonationMutationGraph : ObjectGraphType
    {
        public DonationMutationGraph()
        {
            FieldAsync<NonNullGraphType<StringGraphType>, string>(
                "initializeCreditCardCheckout",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<CreditCardCheckoutInputGraph>>() { Name = "checkout" }),
                resolve: c =>
                {
                    CreditCardCheckout checkout = c.GetArgument<CreditCardCheckout>("checkout");
                    var provider = c.GetUserContext().ServiceProvider;
                    return provider.GetRequiredService<IDonationService>()
                                   .InitializeCreditCardCheckout(checkout, c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<StringGraphType>, string>(
                "initializeSepaDirect",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<SepaDirectCheckoutInputGraph>>() { Name = "checkout" }),
                resolve: async c =>
                {
                    SepaDirectCheckout checkout = c.GetArgument<SepaDirectCheckout>("checkout");
                    var provider = c.GetUserContext().ServiceProvider;
                    await provider.GetRequiredService<IDonationService>()
                                  .InitializeSepaDirect(checkout, c.CancellationToken)
                                  .ConfigureAwait(false);
                    return checkout.SourceId;
                });

            FieldAsync<NonNullGraphType<StringGraphType>, string>(
                "initializeIDealCheckout",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IDealCheckoutInputGraph>>() { Name = "checkout" }),
                resolve: async c =>
                {
                    IDealCheckout checkout = c.GetArgument<IDealCheckout>("checkout");
                    var provider = c.GetUserContext().ServiceProvider;
                    await provider.GetRequiredService<IDonationService>()
                                  .InitializeIDealCheckout(checkout, c.CancellationToken)
                                  .ConfigureAwait(false);
                    return checkout.SourceId;
                });

            FieldAsync<NonNullGraphType<StringGraphType>, string>(
                "cancelSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "subscriptionId" }),
                resolve: async c =>
                {
                    string subscriptionId = c.GetArgument<string>("subscriptionId");
                    var context = c.GetUserContext();
                    await context.ServiceProvider
                                 .GetRequiredService<IDonationService>()
                                 .CancelSubscription(subscriptionId, context.User, c.CancellationToken)
                                 .ConfigureAwait(false);
                    return subscriptionId;
                });
        }
    }
}
