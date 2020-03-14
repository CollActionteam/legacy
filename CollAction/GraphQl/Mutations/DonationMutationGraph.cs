using CollAction.Helpers;
using CollAction.Services.Donation;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Mutations
{
    public sealed class DonationMutationGraph : ObjectGraphType
    {
        public DonationMutationGraph()
        {
            FieldAsync<StringGraphType, string>(
                "initializeCreditCardCheckout",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "currency" },
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "amount" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>>() { Name = "recurring" }),
                resolve: async c =>
                {
                    string currency = c.GetArgument<string>("currency");
                    int amount = c.GetArgument<int>("amount");
                    string name = c.GetArgument<string>("name");
                    string email = c.GetArgument<string>("email");
                    bool recurring = c.GetArgument<bool>("recurring");
                    var provider = c.GetUserContext().ServiceProvider;
                    return await provider.GetRequiredService<IDonationService>()
                                         .InitializeCreditCardCheckout(currency, amount, name, email, recurring, c.CancellationToken)
                                         .ConfigureAwait(false);
                });

            FieldAsync<StringGraphType, string>(
                "initializeSepaDirect",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "sourceId" },
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "amount" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    string sourceId = c.GetArgument<string>("sourceId");
                    int amount = c.GetArgument<int>("amount");
                    string name = c.GetArgument<string>("name");
                    string email = c.GetArgument<string>("email");
                    var provider = c.GetUserContext().ServiceProvider;
                    await provider.GetRequiredService<IDonationService>()
                                  .InitializeSepaDirect(sourceId, name, email, amount, c.CancellationToken)
                                  .ConfigureAwait(false);
                    return sourceId;
                });

            FieldAsync<StringGraphType, string>(
                "initializeIdealCheckout",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "sourceId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    string sourceId = c.GetArgument<string>("sourceId");
                    string name = c.GetArgument<string>("name");
                    string email = c.GetArgument<string>("email");
                    var provider = c.GetUserContext().ServiceProvider;
                    await provider.GetRequiredService<IDonationService>()
                                  .InitializeIdealCheckout(sourceId, name, email, c.CancellationToken)
                                  .ConfigureAwait(false);
                    return sourceId;
                });

            FieldAsync<StringGraphType, string>(
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
