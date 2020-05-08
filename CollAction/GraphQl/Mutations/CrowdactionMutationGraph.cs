using CollAction.GraphQl.Mutations.Input;
using CollAction.GraphQl.Mutations.Result;
using CollAction.GraphQl.Queries;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Crowdactions;
using CollAction.Services.Crowdactions.Models;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CollAction.GraphQl.Mutations
{
    public sealed class CrowdactionMutationGraph : ObjectGraphType
    {
        public CrowdactionMutationGraph()
        {
            FieldAsync<CrowdactionResultGraph, CrowdactionResult>(
                "createCrowdaction",
                arguments: new QueryArguments(
                    new QueryArgument<NewCrowdactionInputGraph>() { Name = "crowdaction" }),
                resolve: async c =>
                {
                    var crowdaction = c.GetArgument<NewCrowdaction>("crowdaction");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<ICrowdactionService>()
                                        .CreateCrowdaction(crowdaction, context.User, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<CrowdactionResultGraph, CrowdactionResult>(
                "updateCrowdaction",
                arguments: new QueryArguments(
                    new QueryArgument<UpdatedCrowdactionInputGraph>() { Name = "crowdaction" }),
                resolve: async c =>
                {
                    var crowdaction = c.GetArgument<UpdatedCrowdaction>("crowdaction");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<ICrowdactionService>()
                                        .UpdateCrowdaction(crowdaction, context.User, c.CancellationToken)
                                        .ConfigureAwait(false);
                }).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<IdGraphType, int>(
                "deleteCrowdaction",
                arguments: new QueryArguments(new QueryArgument<IdGraphType>() { Name = "id" }),
                resolve: c =>
                {
                    var context = c.GetUserContext();
                    int id = c.GetArgument<int>("id");
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .DeleteCrowdaction(id, c.CancellationToken);
                }).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<AddParticipantResultGraph, AddParticipantResult>(
                "commitToCrowdactionLoggedIn",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" }),
                resolve: async c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<ICrowdactionService>()
                                        .CommitToCrowdactionLoggedIn(context.User, crowdactionId, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<AddParticipantResultGraph, AddParticipantResult>(
                "commitToCrowdactionAnonymous",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string email = c.GetArgument<string>("email");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<ICrowdactionService>()
                                        .CommitToCrowdactionAnonymous(email, crowdactionId, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<CrowdactionResultGraph, CrowdactionResult>(
                "sendCrowdactionEmail",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "subject" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "message" }),
                resolve: async c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string subject = c.GetArgument<string>("subject");
                    string message = c.GetArgument<string>("message");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<ICrowdactionService>()
                                        .SendCrowdactionEmail(crowdactionId, subject, message, context.User, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<CrowdactionParticipantGraph, CrowdactionParticipant>(
                "changeCrowdactionSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "token" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>>() { Name = "isSubscribed" }),
                resolve: async c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string userId = c.GetArgument<string>("userId");
                    string token = c.GetArgument<string>("token");
                    bool isSubscribed = c.GetArgument<bool>("isSubscribed");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<ICrowdactionService>()
                                        .SetEmailCrowdactionSubscription(crowdactionId, userId, Guid.Parse(token), isSubscribed, c.CancellationToken)
                                        .ConfigureAwait(false);
                });
        }
    }
}
