using CollAction.GraphQl.Mutations.Input;
using CollAction.GraphQl.Queries;
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
        public ProjectMutationGraph(IServiceScopeFactory serviceScopeFactory)
        {
            FieldAsync<ProjectGraph>(
                "createProject",
                arguments: new QueryArguments(
                    new QueryArgument<NewProjectInputGraph>() { Name = "project" }),
                resolve: async c =>
                {
                    var project = c.GetArgument<NewProject>("project");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IProjectService>().CreateProject(project, ((UserContext)c.UserContext).User);
                    }
                });

            FieldAsync<ProjectGraph>(
                "updateProject",
                arguments: new QueryArguments(
                    new QueryArgument<UpdatedProjectInputGraph>() { Name = "project" }),
                resolve: async c =>
                {
                    var project = c.GetArgument<UpdatedProject>("project");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IProjectService>().UpdateProject(project, ((UserContext)c.UserContext).User);
                    }
                }).AuthorizeWith(Constants.AdminRole);

            FieldAsync<AddParticipantResultGraph>(
                "commitToProject",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string email = c.GetArgument<string>("email");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IProjectService>().CommitToProject(email, projectId, ((UserContext)c.UserContext).User);
                    }
                });

            FieldAsync<BooleanGraphType>(
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
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        await scope.ServiceProvider.GetRequiredService<IProjectService>().SendProjectEmail(projectId, subject, message, ((UserContext)c.UserContext).User);
                        return true;
                    }
                });

            FieldAsync<BooleanGraphType>(
                "changeProjectSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>() { Name = "projectId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "token" }),
                resolve: async c =>
                {
                    int projectId = c.GetArgument<int>("projectId");
                    string userId = c.GetArgument<string>("userId");
                    string token = c.GetArgument<string>("token");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        await scope.ServiceProvider.GetRequiredService<IProjectService>().ChangeProjectSubscription(projectId, userId, Guid.Parse(token));
                        return true;
                    }
                });
        }
    }
}
