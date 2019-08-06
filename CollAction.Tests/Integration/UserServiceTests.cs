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

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public class UserServiceTests : IntegrationTestBase // TODO
    {
        [TestMethod]
        public Task TestPasswordReset()
            => WithServiceProvider(
                ConfigureReplacementServices,
                async scope =>
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var (result, code) = await userService.ForgotPassword("nonexistent@collaction.org");
                    Assert.IsFalse(result.Succeeded);

                    ApplicationUser user = await context.Users.FirstAsync();
                    (result, code) = await userService.ForgotPassword(user.Email);
                    Assert.IsTrue(result.Succeeded);

                    result = await userService.ResetPassword(user.Email, code, "");
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ResetPassword(user.Email, "", "Test_0_tesT");
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ResetPassword(user.Email, code, "Test_0_tesT");
                    Assert.IsTrue(result.Succeeded);

                    var principal = await signInManager.CreateUserPrincipalAsync(user);
                    result = await userService.ChangePassword(principal, "Test_0_tesT", "");
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ChangePassword(new ClaimsPrincipal(), "Test_0_tesT", "Test_1_tesT");
                    Assert.IsFalse(result.Succeeded);
                    result = await userService.ChangePassword(principal, "Test_0_tesT", "Test_1_tesT");
                    Assert.IsTrue(result.Succeeded);
                });

        [TestMethod]
        public Task TestUserManagement()
            => WithServiceProvider(
                ConfigureReplacementServices,
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
                        });
                    var user = result.User;
                    Assert.IsTrue(result.Result.Succeeded);

                    var principal = await signInManager.CreateUserPrincipalAsync(result.User);
                    result = await userService.UpdateUser(
                        new UpdatedUser()
                        {
                            RepresentsNumberUsers = user.RepresentsNumberParticipants,
                            FirstName = GetRandomString(),
                            LastName = GetRandomString(),
                            Email = result.User.Email,
                            IsSubscribedNewsletter = false,
                            Id = result.User.Id
                        }, principal);
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
                        }, principal);
                    Assert.IsFalse(result.Result.Succeeded);

                    var deleteResult = await userService.DeleteUser(user.Id, new ClaimsPrincipal());
                    Assert.IsFalse(deleteResult.Succeeded);
                    deleteResult = await userService.DeleteUser(user.Id, principal);
                    Assert.IsTrue(deleteResult.Succeeded);
                });

        [TestMethod]
        public Task TestFinishRegistration()
            => WithServiceProvider(
                ConfigureReplacementServices,
                async scope =>
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();

                    var currentProject = await context.Projects.Include(p => p.Owner).FirstOrDefaultAsync();
                    string testEmail = GetTestEmail();
                    AddParticipantResult commitResult = await projectService.CommitToProject(testEmail, currentProject.Id, new ClaimsPrincipal(), CancellationToken.None);
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
                        commitResult.PasswordResetToken);
                    Assert.IsTrue(finishRegistrationResult.Succeeded);
                });

        private void ConfigureReplacementServices(ServiceCollection sc)
        {
            sc.AddTransient(s => new Mock<IEmailSender>().Object);
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@collaction.org";

        private string GetRandomString()
            => Guid.NewGuid().ToString();
    }
}
