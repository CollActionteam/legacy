using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Email;
using CollAction.Services.Projects;
using CollAction.Services.Projects.Models;
using CollAction.Services.User;
using CollAction.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Tests.Integration.Service
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class UserServiceTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestPasswordReset()
            => WithServiceProvider(
                async scope =>
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var (result, code) = await userService.ForgotPassword("nonexistent@collaction.org").ConfigureAwait(false);
                    Assert.IsFalse(result.Succeeded);

                    string testEmail = GetTestEmail();
                    UserResult testUserCreation = await userService.CreateUser(
                        new NewUser() 
                        { 
                            Email = testEmail, 
                            FirstName = testEmail, 
                            LastName = testEmail, 
                            IsSubscribedNewsletter = false, 
                            Password = Guid.NewGuid().ToString() 
                        }).ConfigureAwait(false);
                    ApplicationUser user = testUserCreation.User;
                    (result, code) = await userService.ForgotPassword(user.Email).ConfigureAwait(false);
                    Assert.IsTrue(result.Succeeded);
                    Assert.IsNotNull(code);

                    result = await userService.ResetPassword(user.Email, code, "").ConfigureAwait(false);
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ResetPassword(user.Email, "", "Test_0_tesT").ConfigureAwait(false);
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ResetPassword(user.Email, code, "Test_0_tesT").ConfigureAwait(false);
                    Assert.IsTrue(result.Succeeded);

                    var principal = await signInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);
                    result = await userService.ChangePassword(principal, "Test_0_tesT", "").ConfigureAwait(false);
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ChangePassword(new ClaimsPrincipal(), "Test_0_tesT", "Test_1_tesT").ConfigureAwait(false);
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ChangePassword(principal, "Test_0_tesT", "Test_1_tesT").ConfigureAwait(false);
                    Assert.IsTrue(result.Succeeded);
                });

        [TestMethod]
        public Task TestUserManagement()
            => WithServiceProvider(
                async scope =>
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var result = await userService.CreateUser(
                        new NewUser()
                        {
                            Email = GetTestEmail(),
                            FirstName = GetRandomString(),
                            LastName = GetRandomString(),
                            Password = GetRandomString(),
                            IsSubscribedNewsletter = true
                        }).ConfigureAwait(false);
                    var user = result.User;
                    Assert.IsTrue(result.Result.Succeeded);

                    var principal = await signInManager.CreateUserPrincipalAsync(result.User).ConfigureAwait(false);
                    result = await userService.UpdateUser(
                        new UpdatedUser()
                        {
                            RepresentsNumberUsers = user.RepresentsNumberParticipants,
                            FirstName = GetRandomString(),
                            LastName = GetRandomString(),
                            Email = result.User.Email,
                            IsSubscribedNewsletter = false,
                            Id = result.User.Id
                        },
                        principal).ConfigureAwait(false);
                    Assert.IsTrue(result.Result.Succeeded);

                    result = await userService.UpdateUser(
                        new UpdatedUser()
                        {
                            RepresentsNumberUsers = user.RepresentsNumberParticipants + 1,
                            FirstName = GetRandomString(),
                            LastName = GetRandomString(),
                            Email = result.User.Email,
                            IsSubscribedNewsletter = false,
                            Id = result.User.Id
                        },
                        principal).ConfigureAwait(false);
                    Assert.IsFalse(result.Result.Succeeded);

                    var deleteResult = await userService.DeleteUser(user.Id, new ClaimsPrincipal()).ConfigureAwait(false);
                    Assert.IsFalse(deleteResult.Succeeded);
                    deleteResult = await userService.DeleteUser(user.Id, principal).ConfigureAwait(false);
                    Assert.IsTrue(deleteResult.Succeeded);
                });

        [TestMethod]
        public Task TestFinishRegistration()
            => WithServiceProvider(
                async scope =>
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();

                    var currentProject = await context.Projects.Include(p => p.Owner).FirstOrDefaultAsync().ConfigureAwait(false);
                    string testEmail = GetTestEmail();
                    AddParticipantResult commitResult = await projectService.CommitToProject(testEmail, currentProject.Id, new ClaimsPrincipal(), CancellationToken.None).ConfigureAwait(false);
                    Assert.AreEqual(AddParticipantScenario.AnonymousCreatedAndAdded, commitResult.Scenario);

                    var finishRegistrationResult = await userService.FinishRegistration(
                        new NewUser()
                        {
                            Email = testEmail,
                            FirstName = GetRandomString(),
                            LastName = GetRandomString(),
                            IsSubscribedNewsletter = false,
                            Password = "Test_0_tesT"
                        },
                        commitResult.PasswordResetToken).ConfigureAwait(false);
                    Assert.IsTrue(finishRegistrationResult.Result.Succeeded);
                    Assert.IsNotNull(finishRegistrationResult.User);
                });

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            collection.AddTransient(s => new Mock<IEmailSender>().Object);
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@collaction.org";

        private string GetRandomString()
            => Guid.NewGuid().ToString();
    }
}
