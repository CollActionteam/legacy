using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CollAction.Services.Newsletter;
using System;
using Moq;
using Hangfire;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MailChimp.Net;
using MailChimp.Net.Models;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class NewsletterServiceTests
    {
        private NewsletterService newsletterService;
        private Mock<IBackgroundJobClient> jobClient;

        [TestInitialize]
        public void Initialize()
        {
            IConfiguration configuration =
                new ConfigurationBuilder().AddUserSecrets<Startup>()
                                          .AddEnvironmentVariables()
                                          .Build();
            jobClient = new Mock<IBackgroundJobClient>();
            newsletterService = new NewsletterService(
                new MailChimpManager(configuration["MailChimpKey"]),
                new OptionsWrapper<NewsletterServiceOptions>(
                    new NewsletterServiceOptions()
                    {
                        MailChimpNewsletterListId = configuration["MailChimpTestListId"]
                }), 
                new LoggerFactory().CreateLogger<NewsletterService>(), 
                jobClient.Object);
        }

        [TestMethod]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            string email = GetTestEmail();
            Assert.IsFalse(await newsletterService.IsSubscribedAsync(email));
        }

        [TestMethod]
        public async Task TestSubscribeListMemberAsPending()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, true);
                Status status = await newsletterService.GetListMemberStatus(email);
                Assert.AreEqual(Status.Pending, status);
                Assert.IsTrue(await newsletterService.IsSubscribedAsync(email));
            }
            finally
            {
                await newsletterService.UnsubscribeMember(email);
            }
        }

        [TestMethod]
        public async Task TestSubscribeListMemberAsSubscribed()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, false);
                Status status = await newsletterService.GetListMemberStatus(email);
                Assert.AreEqual(Status.Subscribed, status);
                Assert.IsTrue(await newsletterService.IsSubscribedAsync(email));
            }
            finally
            {
                await newsletterService.UnsubscribeMember(email);
            }
        }

        [TestMethod]
        public async Task TestUnsubscribeExistingListMember()
        {
            string email = GetTestEmail();

            try
            {
                await newsletterService.SetSubscription(email, true, true);
                Status status = await newsletterService.GetListMemberStatus(email);
                Assert.AreEqual(Status.Pending, status);

                await newsletterService.SetSubscription(email, false, false);
                Assert.IsFalse(await newsletterService.IsSubscribedAsync(email));
            }
            finally
            {
                await newsletterService.SetSubscription(email, false, false);
            }
        }

        [TestMethod]
        public async Task TestUnsubscribeSubscribeMultiple()
        {
            string email = GetTestEmail();

            try
            {
                for (int attempt = 0; attempt < 4; attempt++)
                {
                    for (bool requireEmail = true; requireEmail; requireEmail = !requireEmail)
                    {
                        await newsletterService.SetSubscription(email, true, requireEmail);
                        Assert.IsTrue(await newsletterService.IsSubscribedAsync(email));

                        await newsletterService.SetSubscription(email, false, requireEmail);
                        Assert.IsFalse(await newsletterService.IsSubscribedAsync(email));
                    }
                }
            }
            finally
            {
                await newsletterService.SetSubscription(email, false, true);
            }
        }

        [TestMethod]
        public async Task TestUnsubscribeNonExistingListMember()
        {
            string email = GetTestEmail();

            await newsletterService.SetSubscription(email, false, false);
            Assert.IsFalse(await newsletterService.IsSubscribedAsync(email));
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@outlook.com";
    }
}
