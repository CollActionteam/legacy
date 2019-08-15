using CollAction.GraphQl.Mutations.Input;
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
    public class ProjectMutationGraph : ObjectGraphType
    {
        public ProjectMutationGraph()
        {
            FieldAsync<ProjectGraph, Project>(
                "createProject",
                arguments: new QueryArguments(
                    new QueryArgument<NewProjectInputGraph>() { Name = "project" }),
                resolve: async c =>
                {
                    var project = c.GetArgument<NewProject>("project");
                    var provider = c.GetUserContext().ServiceProvider;
                    return await provider.GetRequiredService<IProjectService>()
                                         .CreateProject(project, ((UserContext)c.UserContext).User, c.CancellationToken);
                });

            FieldAsync<ProjectGraph, Project>(
                "updateProject",
                arguments: new QueryArguments(
                    new QueryArgument<UpdatedProjectInputGraph>() { Name = "project" }),
                resolve: async c =>
                {
                    var project = c.GetArgument<UpdatedProject>("project");
                    var provider = c.GetUserContext().ServiceProvider;
                    return await provider.GetRequiredService<IProjectService>()
                                         .UpdateProject(project, ((UserContext)c.UserContext).User, c.CancellationToken);
                }).AuthorizeWith(Constants.GraphQlAdminPolicy);

            FieldAsync<AddParticipantResultGraph, AddParticipantResult>(
                "commitToProject",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string email = c.GetArgument<string>("email");
                    var provider = c.GetUserContext().ServiceProvider;
                    return await provider.GetRequiredService<IProjectService>()
                                         .CommitToProject(email, projectId, ((UserContext)c.UserContext).User, c.CancellationToken);
                });

            FieldAsync<ProjectGraph, Project>(
                "sendProjectEmail",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "subject" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "message" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string subject = c.GetArgument<string>("subject");
                    string message = c.GetArgument<string>("message");
                    var provider = c.GetUserContext().ServiceProvider;
                    return await provider.GetRequiredService<IProjectService>()
                                         .SendProjectEmail(projectId, subject, message, ((UserContext)c.UserContext).User, c.CancellationToken);
                });

            FieldAsync<ProjectParticipantGraph, ProjectParticipant>(
                "changeProjectSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "token" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>>() { Name = "isSubscribed" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string userId = c.GetArgument<string>("userId");
                    string token = c.GetArgument<string>("token");
                    bool isSubscribed = c.GetArgument<bool>("isSubscribed");
                    var provider = c.GetUserContext().ServiceProvider;
                    return await provider.GetRequiredService<IProjectService>()
                                         .SetEmailProjectSubscription(projectId, userId, Guid.Parse(token), isSubscribed, c.CancellationToken);
                });
        }
    }
}
