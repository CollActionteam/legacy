using CollAction.Services.Newsletter;
using Hangfire;
using MailChimp.Net.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration.Service
{
    [Trait("Category", "Integration")]
    public sealed class NewsletterServiceTests : IntegrationTestBase
    {
        private readonly INewsletterService newsletterService;

        public NewsletterServiceTests(): base(false)
        {
            newsletterService = Scope.ServiceProvider.GetRequiredService<INewsletterService>();
        }

        [Fact]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            string email = GetTestEmail();
            Assert.False(await newsletterService.IsSubscribedAsync(email));
        }

        [Fact]
        public async Task TestSubscribeListMemberAsPending()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, true);
                Status status = await newsletterService.GetListMemberStatus(email);
                Assert.Equal(Status.Pending, status);
                Assert.True(await newsletterService.IsSubscribedAsync(email));
            }
            finally
            {
                await newsletterService.UnsubscribeMember(email);
            }
        }

        [Fact]
        public async Task TestSubscribeListMemberAsSubscribed()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, false);
                Status status = await newsletterService.GetListMemberStatus(email);
                Assert.Equal(Status.Subscribed, status);
                Assert.True(await newsletterService.IsSubscribedAsync(email));
            }
            finally
            {
                await newsletterService.UnsubscribeMember(email);
            }
        }

        [Fact]
        public async Task TestUnsubscribeExistingListMember()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, true);
                Status status = await newsletterService.GetListMemberStatus(email);
                Assert.Equal(Status.Pending, status);

                await newsletterService.SetSubscription(email, false, false);
                Assert.False(await newsletterService.IsSubscribedAsync(email));
            }
            finally
            {
                await newsletterService.SetSubscription(email, false, false);
            }
        }

        [Fact]
        public async Task TestUnsubscribeSubscribeMultiple()
        {
            string email = GetTestEmail();

            try
            {
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    for (bool requireEmail = true; requireEmail; requireEmail = !requireEmail)
                    {
                        await newsletterService.SetSubscription(email, true, requireEmail);
                        Assert.True(await newsletterService.IsSubscribedAsync(email));

                        await newsletterService.SetSubscription(email, false, requireEmail);
                        Assert.False(await newsletterService.IsSubscribedAsync(email));
                    }
                }
            }
            finally
            {
                await newsletterService.SetSubscription(email, false, true);
            }
        }

        [Fact]
        public async Task TestUnsubscribeNonExistingListMember()
        {
            string email = GetTestEmail();

            await newsletterService.SetSubscription(email, false, false);
            Assert.False(await newsletterService.IsSubscribedAsync(email));
        }

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            var jobClient = new Mock<IBackgroundJobClient>();
            collection.AddScoped(s => jobClient.Object);
        }

        private static string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@collaction.org";
    }
}
