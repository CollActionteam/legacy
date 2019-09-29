﻿using CollAction.GraphQl.Mutations.Input;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.User;
using CollAction.Services.User.Models;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CollAction.GraphQl.Mutations
{
    public class ApplicationUserMutationGraph : ObjectGraphType
    {
        public ApplicationUserMutationGraph()
        {
            FieldAsync<UserResultGraph, UserResult>(
                "createUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<NewUserInputGraph>>() { Name = "user" }),
                resolve: async c =>
                {
                    var newUser = c.GetArgument<NewUser>("user");
                    var provider = c.GetUserContext().ServiceProvider;
                    var userService = provider.GetRequiredService<IUserService>();
                    var signInManager = provider.GetRequiredService<SignInManager<ApplicationUser>>();

                    var result = await userService.CreateUser(newUser);
                    if (result.Result.Succeeded)
                    {
                        await signInManager.SignInAsync(result.User, isPersistent: true);
                    }

                    return result;
                });
            
            FieldAsync<UserResultGraph, UserResult>(
                "updateUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<UpdatedUserInputGraph>>() { Name = "user" }),
                resolve: async c =>
                {
                    var updatedUser = c.GetArgument<UpdatedUser>("user");
                    var context = c.GetUserContext();
                    var userService = context.ServiceProvider.GetRequiredService<IUserService>();
                    return await userService.UpdateUser(updatedUser, context.User);
                });
            
            FieldAsync<IdentityResultGraph, IdentityResult>(
                "deleteUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "userId" }),
                resolve: async c =>
                {
                    string userId = c.GetArgument<string>("userId");
                    var context = c.GetUserContext();
                    var userService = context.ServiceProvider.GetRequiredService<IUserService>();
                    return await userService.DeleteUser(userId, context.User);
                });

            FieldAsync<IdentityResultGraph, IdentityResult>(
                "changePassword",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "currentPassword" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "newPassword" }),
                resolve: async c =>
                {
                    string currentPassword = c.GetArgument<string>("currentPassword");
                    string newPassword = c.GetArgument<string>("newPassword");
                    var context = c.GetUserContext();
                    var userService = context.ServiceProvider.GetRequiredService<IUserService>();
                    return await userService.ChangePassword(context.User, currentPassword, newPassword);
                });

            FieldAsync<UserResultGraph, UserResult>(
                "finishRegistration",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<NewUserInputGraph>>() { Name = "user" },
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "code" }),
                resolve: async c =>
                {
                    var newUser = c.GetArgument<NewUser>("user");
                    string code = c.GetArgument<string>("code");
                    var provider = c.GetUserContext().ServiceProvider;
                    var userService = provider.GetRequiredService<IUserService>();
                    var result = await userService.FinishRegistration(newUser, code);

                    if (result.Result.Succeeded)
                    {
                        await provider.GetRequiredService<SignInManager<ApplicationUser>>()
                                      .SignInAsync(result.User, isPersistent: true);
                    }

                    return result;
                });

            FieldAsync<IdentityResultGraph, IdentityResult>(
                "forgotPassword",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() { Name = "email" }),
                resolve: async c =>
                {
                    string email = c.GetArgument<string>("email");
                    var provider = c.GetUserContext().ServiceProvider;
                    var userService = provider.GetRequiredService<IUserService>();
                    return (await userService.ForgotPassword(email)).Result;
                });

            FieldAsync<IdentityResultGraph, IdentityResult>(
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
                    var provider = c.GetUserContext().ServiceProvider;
                    var userService = provider.GetRequiredService<IUserService>();
                    return await userService.ResetPassword(email, code, password);
                });

            FieldAsync<IntGraphType, int>(
                "ingestUserEvent",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "eventData" },
                    new QueryArgument<NonNullGraphType<BooleanGraphType>> { Name = "canTrack" }),
                resolve: async c =>
                {
                    JObject eventData = JObject.Parse(c.GetArgument<string>("eventData"));
                    bool canTrack = c.GetArgument<bool>("canTrack");
                    var context = c.GetUserContext();
                    return await context.ServiceProvider.GetRequiredService<IUserService>().IngestUserEvent(context.User, eventData, canTrack, c.CancellationToken);
                });
        }
    }
}