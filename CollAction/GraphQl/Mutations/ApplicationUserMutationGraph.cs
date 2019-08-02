using CollAction.GraphQl.Mutations.Input;
using CollAction.Services.User;
using CollAction.Services.User.Models;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CollAction.GraphQl.Mutations
{
    public class ApplicationUserMutationGraph : ObjectGraphType
    {
        public ApplicationUserMutationGraph(IServiceScopeFactory serviceScopeFactory)
        {
            FieldAsync<UserResultGraph>(
                "createUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<NewUserInputGraph>>() { Name = "user" }),
                resolve: async c =>
                {
                    var newUser = c.GetArgument<NewUser>("user");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        return await userService.CreateUser(newUser);
                    }
                });
            
            FieldAsync<UserResultGraph>(
                "updateUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<UpdatedUserInputGraph>>() { Name = "user" }),
                resolve: async c =>
                {
                    var updatedUser = c.GetArgument<UpdatedUser>("user");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        return await userService.UpdateUser(updatedUser, ((UserContext)c.UserContext).User);
                    }
                });

            FieldAsync<IdentityResultGraph>(
                "changePassword",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "currentPassword" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "newPassword" }),
                resolve: async c =>
                {
                    string currentPassword = c.GetArgument<string>("currentPassword");
                    string newPassword = c.GetArgument<string>("newPassword");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        return await userService.ChangePassword(((UserContext)c.UserContext).User, currentPassword, newPassword);
                    }
                });

            FieldAsync<IdentityResultGraph>(
                "forgotPassword",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    string email = c.GetArgument<string>("email");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        return await userService.ForgotPassword(email);
                    }
                });

            FieldAsync<IdentityResultGraph>(
                "resetPassword",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "code" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "password" }),
                resolve: async c =>
                {
                    string email = c.GetArgument<string>("email");
                    string code = c.GetArgument<string>("code");
                    string password = c.GetArgument<string>("password");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        return await userService.ResetPassword(email, code, password);
                    }
                });

            FieldAsync<IntGraphType>(
                "ingestUserEvent",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>{ Name = "eventData" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>> { Name = "canTrack" }),
                resolve: async c =>
                {
                    JObject eventData = JObject.Parse(c.GetArgument<string>("eventData"));
                    bool canTrack = c.GetArgument<bool>("canTrack");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return await scope.ServiceProvider.GetRequiredService<IUserService>().IngestUserEvent(((UserContext)c.UserContext).User, eventData, canTrack);
                    }
                });
        }
    }
}
