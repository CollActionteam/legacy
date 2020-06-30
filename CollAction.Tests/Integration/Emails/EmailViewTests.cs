using CollAction.Models;
using CollAction.Services.Crowdactions.Models;
using CollAction.Tests.Integration;
using CollAction.ViewModels.Email;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using System;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace CollAction.Tests.Unit
{
    [Trait("Category", "Unit")]
    [UsesVerify]
    public class EmailViewTests : IntegrationTestBase
    {
        private readonly IRazorLightEngine engine;

        public EmailViewTests()
        {
            engine = Scope.ServiceProvider.GetRequiredService<IRazorLightEngine>();
        }

        [Fact]
        public async Task VerifyCrowdactionAddedAdmin()
            => await Verifier.Verify(await engine.CompileRenderAsync("CrowdactionAddedAdmin.cshtml", "test"));

        [Fact]
        public async Task VerifyCrowdactionApproval()
            => await Verifier.Verify(await engine.CompileRenderAsync("CrowdactionApproval.cshtml", new object()));

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public async Task VerifyCrowdactionCommit(bool userCreated, bool userAdded, bool userAlreadyActive)
        {
            var settings = new VerifySettings();
            settings.UseParameters(userCreated, userAdded, userAlreadyActive);
            await Verifier.Verify(await engine.CompileRenderAsync("CrowdactionCommit.cshtml", new CrowdactionCommitEmailViewModel(
                new Crowdaction("test", CrowdactionStatus.Running, null, 1, DateTime.UtcNow, DateTime.UtcNow.AddDays(3), "", "", "", null, null),
                new AddParticipantResult(userCreated, userAdded, userAlreadyActive, "test@example.com", "test"),
                new ApplicationUser("test@example.com", DateTime.UtcNow),
                new Uri("https://public.example.com"),
                new Uri("https://crowd.example.com"))), settings);
        }

        [Fact]
        public async Task VerifyCrowdactionConfirmation()
            => await Verifier.Verify(await engine.CompileRenderAsync("CrowdactionConfirmation.cshtml", new object()));

        [Fact]
        public async Task VerifyCrowdactionFailed()
            => await Verifier.Verify(await engine.CompileRenderAsync("CrowdactionFailed.cshtml", new object()));

        [Fact]
        public async Task VerifyCrowdactionSuccess()
            => await Verifier.Verify(await engine.CompileRenderAsync("CrowdactionSuccess.cshtml", new object()));

        [Fact]
        public async Task VerifyDeleteAccount()
            => await Verifier.Verify(await engine.CompileRenderAsync("DeleteAccount.cshtml", new object()));

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task VerifyDonationThankYou(bool hasSubscriptions)
        {
            var settings = new VerifySettings();
            settings.UseParameters(hasSubscriptions);
            await Verifier.Verify(await engine.CompileRenderAsync("DonationThankYou.cshtml", hasSubscriptions), settings);
        }

        [Fact]
        public async Task VerifyResetPassword()
            => await Verifier.Verify(await engine.CompileRenderAsync("ResetPassword.cshtml", new Uri("https://www.example.com")));

        [Fact]
        public async Task VerifyUserCreated()
            => await Verifier.Verify(await engine.CompileRenderAsync("UserCreated.cshtml", new object()));
    }
}
