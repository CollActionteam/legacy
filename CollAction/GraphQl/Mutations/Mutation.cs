using CollAction.Data;
using CollAction.GraphQl.Queries;
using CollAction.Models;
using CollAction.Services.Newsletter;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CollAction.GraphQl.Mutations
{
    public class Mutation : ObjectGraphType
    {
        public Mutation(IServiceScopeFactory serviceScopeFactory)
        {
            FieldAsync<ApplicationUserGraph>(
                "CreateNewUser",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<NewUserGraph>>() { Name = "user" }),
                resolve: async c =>
                {
                    var newUser = c.GetArgument<NewUser>("user");
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var user = new ApplicationUser()
                        {
                            Email = newUser.Email,
                            FirstName = newUser.FirstName,
                            LastName = newUser.LastName
                        };
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        IdentityResult result = await userManager.CreateAsync(user, newUser.Password);
                        if (result.Succeeded)
                        {
                            var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                            newsletterService.SetSubscriptionBackground(newUser.Email, newUser.SubscribeNewsletter);
                            var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                            await signInManager.SignInAsync(user, isPersistent: false);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Error creating user: {string.Join(",", result.Errors.Select(e => e.Description))}");
                        }

                        return user;
                    }
                });
        }
    }
}