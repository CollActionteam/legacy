using CollAction.GraphQl.Mutations.Input;
using CollAction.GraphQl.Mutations.Result;
using CollAction.GraphQl.Queries;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Projects;
using CollAction.Services.Projects.Models;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CollAction.GraphQl.Mutations
{
    public sealed class ProjectMutationGraph : ObjectGraphType
    {
        public ProjectMutationGraph()
        {
            FieldAsync<ProjectResultGraph, ProjectResult>(
                "createProject",
                arguments: new QueryArguments(
                    new QueryArgument<NewProjectInputGraph>() { Name = "project" }),
                resolve: async c =>
                {
                    var project = c.GetArgument<NewProject>("project");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<IProjectService>()
                                        .CreateProject(project, context.User, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<ProjectResultGraph, ProjectResult>(
                "updateProject",
                arguments: new QueryArguments(
                    new QueryArgument<UpdatedProjectInputGraph>() { Name = "project" }),
                resolve: async c =>
                {
                    var project = c.GetArgument<UpdatedProject>("project");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<IProjectService>()
                                        .UpdateProject(project, context.User, c.CancellationToken)
                                        .ConfigureAwait(false);
                }).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);

            FieldAsync<AddParticipantResultGraph, AddParticipantResult>(
                "commitToProjectLoggedIn",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "projectId" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<IProjectService>()
                                        .CommitToProjectLoggedIn(context.User, projectId, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<AddParticipantResultGraph, AddParticipantResult>(
                "commitToProjectAnonymous",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string email = c.GetArgument<string>("email");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<IProjectService>()
                                        .CommitToProjectAnonymous(email, projectId, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<ProjectResultGraph, ProjectResult>(
                "sendProjectEmail",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "subject" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "message" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string subject = c.GetArgument<string>("subject");
                    string message = c.GetArgument<string>("message");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<IProjectService>()
                                        .SendProjectEmail(projectId, subject, message, context.User, c.CancellationToken)
                                        .ConfigureAwait(false);
                });

            FieldAsync<ProjectParticipantGraph, ProjectParticipant>(
                "changeProjectSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "token" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>>() { Name = "isSubscribed" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string userId = c.GetArgument<string>("userId");
                    string token = c.GetArgument<string>("token");
                    bool isSubscribed = c.GetArgument<bool>("isSubscribed");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider
                                        .GetRequiredService<IProjectService>()
                                        .SetEmailProjectSubscription(projectId, userId, Guid.Parse(token), isSubscribed, c.CancellationToken)
                                        .ConfigureAwait(false);
                });
        }
    }
}
