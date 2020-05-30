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
            Assert.False(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
        }

        [Fact]
        public async Task TestSubscribeListMemberAsPending()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, true).ConfigureAwait(false);
                Status status = await newsletterService.GetListMemberStatus(email).ConfigureAwait(false);
                Assert.Equal(Status.Pending, status);
                Assert.True(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
            }
            finally
            {
                await newsletterService.UnsubscribeMember(email).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestSubscribeListMemberAsSubscribed()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, false).ConfigureAwait(false);
                Status status = await newsletterService.GetListMemberStatus(email).ConfigureAwait(false);
                Assert.Equal(Status.Subscribed, status);
                Assert.True(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
            }
            finally
            {
                await newsletterService.UnsubscribeMember(email).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestUnsubscribeExistingListMember()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, true).ConfigureAwait(false);
                Status status = await newsletterService.GetListMemberStatus(email).ConfigureAwait(false);
                Assert.Equal(Status.Pending, status);

                await newsletterService.SetSubscription(email, false, false).ConfigureAwait(false);
                Assert.False(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
            }
            finally
            {
                await newsletterService.SetSubscription(email, false, false).ConfigureAwait(false);
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
                        await newsletterService.SetSubscription(email, true, requireEmail).ConfigureAwait(false);
                        Assert.True(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));

                        await newsletterService.SetSubscription(email, false, requireEmail).ConfigureAwait(false);
                        Assert.False(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
                    }
                }
            }
            finally
            {
                await newsletterService.SetSubscription(email, false, true).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestUnsubscribeNonExistingListMember()
        {
            string email = GetTestEmail();

            await newsletterService.SetSubscription(email, false, false).ConfigureAwait(false);
            Assert.False(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
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
