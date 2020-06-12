using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Crowdactions;
using CollAction.Services.Crowdactions.Models;
using CollAction.Services.Email;
using CollAction.Services.User;
using CollAction.Services.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration.Service
{
    [Trait("Category", "Integration")]
    public sealed class UserServiceTests : IntegrationTestBase
    {
        private readonly IUserService userService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationDbContext context;
        private readonly ICrowdactionService crowdactionService;

        public UserServiceTests() : base(false)
        {
            userService = Scope.ServiceProvider.GetRequiredService<IUserService>();
            signInManager = Scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            crowdactionService = Scope.ServiceProvider.GetRequiredService<ICrowdactionService>();
        }

        [Fact]
        public async Task TestPasswordReset()
        {
            var (result, code) = await userService.ForgotPassword("nonexistent@collaction.org").ConfigureAwait(false);
            Assert.False(result.Succeeded);
            Assert.Null(code);

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
            Assert.True(result.Succeeded);
            Assert.NotNull(code);

            result = await userService.ResetPassword(user.Email, code, "").ConfigureAwait(false);
            Assert.False(result.Succeeded);
            result = await userService.ResetPassword(user.Email, "", "Test_0_tesT").ConfigureAwait(false);
            Assert.False(result.Succeeded);
            result = await userService.ResetPassword(user.Email, code, "Test_0_tesT").ConfigureAwait(false);
            Assert.True(result.Succeeded);

            var principal = await signInManager.CreateUserPrincipalAsync(user).ConfigureAwait(false);
            result = await userService.ChangePassword(principal, "Test_0_tesT", "").ConfigureAwait(false);
            Assert.False(result.Succeeded);
            result = await userService.ChangePassword(new ClaimsPrincipal(), "Test_0_tesT", "Test_1_tesT").ConfigureAwait(false);
            Assert.False(result.Succeeded);
            result = await userService.ChangePassword(principal, "Test_0_tesT", "Test_1_tesT").ConfigureAwait(false);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task TestUserManagement()
        {
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
            Assert.True(result.Result.Succeeded);

            var principal = await signInManager.CreateUserPrincipalAsync(result.User).ConfigureAwait(false);
            result = await userService.UpdateUser(
                new UpdatedUser()
                {
                    representsNumberParticipants = user.RepresentsNumberParticipants,
                    FirstName = GetRandomString(),
                    LastName = GetRandomString(),
                    Email = result.User.Email,
                    IsSubscribedNewsletter = false,
                    Id = result.User.Id
                },
                principal).ConfigureAwait(false);
            Assert.True(result.Result.Succeeded);

            result = await userService.UpdateUser(
                new UpdatedUser()
                {
                    representsNumberParticipants = user.RepresentsNumberParticipants + 1,
                    FirstName = GetRandomString(),
                    LastName = GetRandomString(),
                    Email = result.User.Email,
                    IsSubscribedNewsletter = false,
                    Id = result.User.Id
                },
                principal).ConfigureAwait(false);
            Assert.False(result.Result.Succeeded);

            var deleteResult = await userService.DeleteUser(user.Id, new ClaimsPrincipal()).ConfigureAwait(false);
            Assert.False(deleteResult.Succeeded);
            deleteResult = await userService.DeleteUser(user.Id, principal).ConfigureAwait(false);
            Assert.True(deleteResult.Succeeded);
        }

        [Fact]
        public async Task TestFinishRegistration()
        {
            // Setup
            var crowdaction = new Crowdaction($"test-{Guid.NewGuid()}", CrowdactionStatus.Running, await context.Users.Select(u => u.Id).FirstAsync().ConfigureAwait(false), 10, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), "t", "t", "t", "t", null, null);
            context.Crowdactions.Add(crowdaction);
            await context.SaveChangesAsync().ConfigureAwait(false);

            // Test
            string testEmail = GetTestEmail();
            AddParticipantResult commitResult = await crowdactionService.CommitToCrowdactionAnonymous(testEmail, crowdaction.Id, CancellationToken.None).ConfigureAwait(false);
            Assert.Equal(AddParticipantScenario.AnonymousCreatedAndAdded, commitResult.Scenario);

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
            Assert.True(finishRegistrationResult.Result.Succeeded);
            Assert.NotNull(finishRegistrationResult.User);
        }

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            collection.AddTransient(s => new Mock<IEmailSender>().Object);
        }

        private static string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@collaction.org";

        private static string GetRandomString()
            => Guid.NewGuid().ToString();
    }
}
