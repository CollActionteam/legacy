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
            FieldAsync<NonNullGraphType<CrowdactionResultGraph>, CrowdactionResult>(
                "createCrowdaction",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<NewCrowdactionInputGraph>>() { Name = "crowdaction" }),
                resolve: c =>
                {
                    var crowdaction = c.GetArgument<NewCrowdaction>("crowdaction");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .CreateCrowdaction(crowdaction, context.User, c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<CrowdactionResultGraph>, CrowdactionResult>(
                "updateCrowdaction",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<UpdatedCrowdactionInputGraph>>() { Name = "crowdaction" }),
                resolve: c =>
                {
                    var crowdaction = c.GetArgument<UpdatedCrowdaction>("crowdaction");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .UpdateCrowdaction(crowdaction, c.CancellationToken);
                }).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<NonNullGraphType<IdGraphType>, int>(
                "deleteCrowdaction",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "id" }),
                resolve: c =>
                {
                    var context = c.GetUserContext();
                    int id = c.GetArgument<int>("id");
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .DeleteCrowdaction(id, c.CancellationToken);
                }).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<NonNullGraphType<AddParticipantResultGraph>, AddParticipantResult>(
                "commitToCrowdactionLoggedIn",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" }),
                resolve: c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .CommitToCrowdactionLoggedIn(context.User, crowdactionId, c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<AddParticipantResultGraph>, AddParticipantResult>(
                "commitToCrowdactionAnonymous",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string email = c.GetArgument<string>("email");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .CommitToCrowdactionAnonymous(email, crowdactionId, c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<CrowdactionCommentGraph>, CrowdactionComment>(
                "createComment",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "comment" }),
                resolve: c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string comment = c.GetArgument<string>("comment");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .CreateComment(comment, crowdactionId, context.User, c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<CrowdactionCommentGraph>, CrowdactionComment>(
                "createCommentAnonymous",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "comment" }),
                resolve: c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string comment = c.GetArgument<string>("comment");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .CreateCommentAnonymous(comment, crowdactionId, c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<CrowdactionCommentGraph>, CrowdactionComment>(
                "approveComment",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "commentId" }),
                resolve: c =>
                {
                    int commentId = c.GetArgument<int>("commentId");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .ApproveComment(commentId, c.CancellationToken);
                }).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<NonNullGraphType<IntGraphType>, int>(
                "deleteComment",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "commentId" }),
                resolve: async c =>
                {
                    int commentId = c.GetArgument<int>("commentId");
                    var context = c.GetUserContext();
                    await context.ServiceProvider
                                 .GetRequiredService<ICrowdactionService>()
                                 .DeleteComment(commentId, c.CancellationToken)
                                 .ConfigureAwait(false);
                    return commentId;
                }).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<NonNullGraphType<CrowdactionResultGraph>, CrowdactionResult>(
                "sendCrowdactionEmail",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "subject" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "message" }),
                resolve: c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string subject = c.GetArgument<string>("subject");
                    string message = c.GetArgument<string>("message");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .SendCrowdactionEmail(crowdactionId, subject, message, context.User, c.CancellationToken);
                });

            FieldAsync<NonNullGraphType<CrowdactionParticipantGraph>, CrowdactionParticipant>(
                "changeCrowdactionSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "crowdactionId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "token" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>>() { Name = "isSubscribed" }),
                resolve: c =>
                {
                    int crowdactionId = c.GetArgument<int>("crowdactionId");
                    string userId = c.GetArgument<string>("userId");
                    string token = c.GetArgument<string>("token");
                    bool isSubscribed = c.GetArgument<bool>("isSubscribed");
                    var context = c.GetUserContext();
                    return context.ServiceProvider
                                  .GetRequiredService<ICrowdactionService>()
                                  .SetEmailCrowdactionSubscription(crowdactionId, userId, Guid.Parse(token), isSubscribed, c.CancellationToken);
                });
        }
    }
}
