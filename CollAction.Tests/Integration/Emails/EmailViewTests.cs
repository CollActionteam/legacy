using CollAction.Models;
using CollAction.Services.Crowdactions.Models;
using CollAction.Tests.Integration;
using CollAction.ViewModels.Email;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using System;
using System.Threading.Tasks;
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

        [Fact]
        public async Task VerifyCrowdactionCommit()
            => await Verifier.Verify(await engine.CompileRenderAsync("CrowdactionCommit.cshtml", new CrowdactionCommitEmailViewModel(
                new Crowdaction("test", CrowdactionStatus.Running, null, 1, DateTime.UtcNow, DateTime.UtcNow.AddDays(3), "", "", "", null, null),
                new AddParticipantResult(true, true),
                new ApplicationUser("test@example.com", DateTime.UtcNow),
                new Uri("https://public.example.com"),
                new Uri("https://crowd.example.com"))));

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

        [Fact]
        public async Task VerifyDonationThankYou()
            => await Verifier.Verify(await engine.CompileRenderAsync("DonationThankYou.cshtml", false));

        [Fact]
        public async Task VerifyResetPassword()
            => await Verifier.Verify(await engine.CompileRenderAsync("ResetPassword.cshtml", new Uri("https://www.example.com")));

        [Fact]
        public async Task VerifyUserCreated()
            => await Verifier.Verify(await engine.CompileRenderAsync("UserCreated.cshtml", new object()));
    }
}
