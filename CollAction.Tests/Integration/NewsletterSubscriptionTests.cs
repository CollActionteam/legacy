using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CollAction.Services.Newsletter;
using System;
using Moq;
using Hangfire;
using Hangfire.States;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MailChimp.Net;
using MailChimp.Net.Models;
using MailChimp.Net.Core;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class NewsletterSubscriptionServiceTests
    {
        private NewsletterSubscriptionService _newsletterSubscriptionService;
        private Mock<IBackgroundJobClient> _jobClient;

        [TestInitialize]
        public void Initialize()
        {
            IConfiguration configuration =
                new ConfigurationBuilder().AddUserSecrets<Startup>()
                                          .AddEnvironmentVariables()
                                          .Build();
            _jobClient = new Mock<IBackgroundJobClient>();
            _newsletterSubscriptionService = new NewsletterSubscriptionService(
                new MailChimpManager(configuration["MailChimpKey"]),
                new OptionsWrapper<NewsletterSubscriptionServiceOptions>(
                    new NewsletterSubscriptionServiceOptions()
                    {
                        MailChimpNewsletterListId = configuration["MailChimpTestListId"]
                }), 
                new LoggerFactory().CreateLogger<NewsletterSubscriptionService>(), 
                _jobClient.Object);
        }

        [TestMethod]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            string email = GetTestEmail();
            Assert.IsFalse(await _newsletterSubscriptionService.IsSubscribedAsync(email));
        }

        [TestMethod]
        public async Task TestSubscribeListMemberAsPending()
        {
            string email = GetTestEmail();

            try
            {
                await _newsletterSubscriptionService.SetSubscription(email, true, true);
                Status status = await _newsletterSubscriptionService.GetListMemberStatus(email);
                Assert.AreEqual(Status.Pending, status);
                Assert.IsTrue(await _newsletterSubscriptionService.IsSubscribedAsync(email));
            }
            finally
            {
                await _newsletterSubscriptionService.UnsubscribeMember(email);
            }
        }

        [TestMethod]
        public async Task TestSubscribeListMemberAsSubscribed()
        {
            string email = GetTestEmail();

            try
            {
                await _newsletterSubscriptionService.SetSubscription(email, true, false);
                Status status = await _newsletterSubscriptionService.GetListMemberStatus(email);
                Assert.AreEqual(Status.Subscribed, status);
                Assert.IsTrue(await _newsletterSubscriptionService.IsSubscribedAsync(email));
            }
            finally
            {
                await _newsletterSubscriptionService.UnsubscribeMember(email);
            }
        }

        [TestMethod]
        public async Task TestUnsubscribeExistingListMember()
        {
            string email = GetTestEmail();

            try
            {
                await _newsletterSubscriptionService.SetSubscription(email, true, true);
                Status status = await _newsletterSubscriptionService.GetListMemberStatus(email);
                Assert.AreEqual(Status.Pending, status);

                await _newsletterSubscriptionService.SetSubscription(email, false, false);
                Assert.IsFalse(await _newsletterSubscriptionService.IsSubscribedAsync(email));
            }
            finally
            {
                await _newsletterSubscriptionService.SetSubscription(email, false, false);
            }
        }

        [TestMethod]
        public async Task TestUnsubscribeSubscribeMultiple()
        {
            string email = GetTestEmail();

            try
            {
                await _newsletterSubscriptionService.SetSubscription(email, true, true);
                Assert.IsTrue(await _newsletterSubscriptionService.IsSubscribedAsync(email));
                await _newsletterSubscriptionService.SetSubscription(email, false, true);
                Assert.IsFalse(await _newsletterSubscriptionService.IsSubscribedAsync(email));
                await _newsletterSubscriptionService.SetSubscription(email, true, true);
                Assert.IsTrue(await _newsletterSubscriptionService.IsSubscribedAsync(email));
                await _newsletterSubscriptionService.SetSubscription(email, false, true);
                Assert.IsFalse(await _newsletterSubscriptionService.IsSubscribedAsync(email));
            }
            finally
            {
                await _newsletterSubscriptionService.SetSubscription(email, false, true);
            }
        }

        [TestMethod]
        public async Task TestUnsubscribeNonExistingListMember()
        {
            string email = GetTestEmail();

            await _newsletterSubscriptionService.SetSubscription(email, false, false);
            Assert.IsFalse(await _newsletterSubscriptionService.IsSubscribedAsync(email));
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@outlook.com";
    }
}
