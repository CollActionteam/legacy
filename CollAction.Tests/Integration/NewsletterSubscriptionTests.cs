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
            _jobClient.Setup(jc => jc.Create(It.IsAny<Hangfire.Common.Job>(), It.IsAny<IState>())) // Run the job immediately for easier testing
                      .Returns<Hangfire.Common.Job, IState>(
                          (job, state) => {
                              try
                              {
                                  Task.Run(() => (Task)job.Method.Invoke(_newsletterSubscriptionService, job.Args.ToArray())).Wait();
                              }
                              catch (Exception)
                              {
                                  // Hangfire will only log exceptions
                              }
                              return string.Empty;
                          });
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
        public async Task TestAddListMemberAsPending()
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
                await _newsletterSubscriptionService.DeleteListMember(email);
            }
        }

        [TestMethod]
        public async Task TestAddListMemberAsSubscribed()
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
                await _newsletterSubscriptionService.DeleteListMember(email);
            }
        }

        [TestMethod]
        public async Task TestDeleteExistingListMember()
        {
            string email = GetTestEmail();

            await _newsletterSubscriptionService.SetSubscription(email, true, true);
            Status status = await _newsletterSubscriptionService.GetListMemberStatus(email);
            Assert.AreEqual(Status.Pending, status);

            await _newsletterSubscriptionService.SetSubscription(email, false, false);
            await Assert.ThrowsExceptionAsync<MailChimpNotFoundException>(() => _newsletterSubscriptionService.GetListMemberStatus(email));
            Assert.IsFalse(await _newsletterSubscriptionService.IsSubscribedAsync(email));
        }

        [TestMethod]
        public async Task TestDeleteNonExistingListMember()
        {
            string email = GetTestEmail();

            await _newsletterSubscriptionService.SetSubscription(email, false, false);
            await Assert.ThrowsExceptionAsync<MailChimpNotFoundException>(() => _newsletterSubscriptionService.GetListMemberStatus(email));
            Assert.IsFalse(await _newsletterSubscriptionService.IsSubscribedAsync(email));
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@outlook.com";
    }
}
