using CollAction.Services.Donation;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Mutations
{
    public class DonationMutationGraph : ObjectGraphType
    {
        public DonationMutationGraph(IServiceScopeFactory serviceScopeFactory)
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
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IDonationService>().InitializeCreditCardCheckout(currency, amount, name, email, recurring, c.CancellationToken);
                    }
                });

            FieldAsync<StringGraphType, string>(
                "initializeSepaDirect",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "sourceId" },
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "amount" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    string sourceId = c.GetArgument<string>("sourceId");
                    int amount = c.GetArgument<int>("amount");
                    string name = c.GetArgument<string>("name");
                    string email = c.GetArgument<string>("email");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        await scope.ServiceProvider.GetRequiredService<IDonationService>().InitializeSepaDirect(sourceId, name, email, amount, c.CancellationToken);
                        return sourceId;
                    }
                });

            FieldAsync<StringGraphType, string>(
                "initializeIdealCheckout",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "sourceId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    string sourceId = c.GetArgument<string>("sourceId");
                    string name = c.GetArgument<string>("name");
                    string email = c.GetArgument<string>("email");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        await scope.ServiceProvider.GetRequiredService<IDonationService>().InitializeIdealCheckout(sourceId, name, email, c.CancellationToken);
                        return sourceId;
                    }
                });

            FieldAsync<StringGraphType, string>(
                "cancelSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "subscriptionId" }),
                resolve: async c =>
                {
                    string subscriptionId = c.GetArgument<string>("subscriptionId");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        await scope.ServiceProvider.GetRequiredService<IDonationService>().CancelSubscription(subscriptionId, ((UserContext)c.UserContext).User, c.CancellationToken);
                        return subscriptionId;
                    }
                });
        }
    }
}
