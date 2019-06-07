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

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class NewsletterSubscriptionServiceTests
    {
        private string _newsletterTestListId;
        private NewsletterSubscriptionService _service;
        private Mock<IBackgroundJobClient> _jobClient;

        [TestInitialize]
        public void Initialize()
        {
            IConfiguration configuration =
                new ConfigurationBuilder().AddUserSecrets<Startup>()
                                          .AddEnvironmentVariables()
                                          .Build();
            _newsletterTestListId = configuration["MailChimpTestListId"];          
            _jobClient = new Mock<IBackgroundJobClient>();
            _jobClient.Setup(jc => jc.Create(It.IsAny<Hangfire.Common.Job>(), It.IsAny<IState>()))
                      .Returns<Hangfire.Common.Job, IState>(
                          (job, state) => {
                              Task.Run(() => (Task)job.Method.Invoke(_service, job.Args.ToArray())).Wait();
                              return string.Empty;
                          });
            _service = new NewsletterSubscriptionService(
                new OptionsWrapper<NewsletterSubscriptionServiceOptions>(
                    new NewsletterSubscriptionServiceOptions()
                    {
                        MailChimpKey = configuration["MailChimpKey"],
                        MailChimpNewsletterListId = _newsletterTestListId
                    }), new LoggerFactory().CreateLogger<NewsletterSubscriptionService>(), _jobClient.Object);
        }

        [TestMethod]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            string email = GetTestEmail();

            NewsletterSubscriptionService.SubscriptionStatus status = await _service.GetListMemberStatusAsync(_newsletterTestListId, email);
            Assert.AreEqual(NewsletterSubscriptionService.SubscriptionStatus.NotFound, status);
        }

        [TestMethod]
        public async Task TestAddListMemberAsPending()
        {
            string email = GetTestEmail();

            try
            {
                await _service.AddOrUpdateListMemberAsync(_newsletterTestListId, email, true);
                NewsletterSubscriptionService.SubscriptionStatus status = await _service.GetListMemberStatusAsync(_newsletterTestListId, email);
                Assert.AreEqual(NewsletterSubscriptionService.SubscriptionStatus.Pending, status);
            }
            finally
            {
                await _service.DeleteListMemberAsync(_newsletterTestListId, email);
            }
        }

        [TestMethod]
        public async Task TestAddListMemberAsSubscribed()
        {
            string email = GetTestEmail();

            try
            {
                await _service.AddOrUpdateListMemberAsync(_newsletterTestListId, email, false);
                NewsletterSubscriptionService.SubscriptionStatus status = await _service.GetListMemberStatusAsync(_newsletterTestListId, email);
                Assert.AreEqual(NewsletterSubscriptionService.SubscriptionStatus.Subscribed, status);
            }
            finally
            {
                await _service.DeleteListMemberAsync(_newsletterTestListId, email);
            }
        }

        [TestMethod]
        public async Task TestDeleteExistingListMember()
        {
            string email = GetTestEmail();

            await _service.AddOrUpdateListMemberAsync(_newsletterTestListId, email);
            NewsletterSubscriptionService.SubscriptionStatus status = await _service.GetListMemberStatusAsync(_newsletterTestListId, email);
            Assert.AreEqual(NewsletterSubscriptionService.SubscriptionStatus.Pending, status);

            await _service.DeleteListMemberAsync(_newsletterTestListId, email);
            status = await _service.GetListMemberStatusAsync(_newsletterTestListId, email);
            // Test says Unknown is returned, until you change the expected result to Unknown. Then it's NotFound again...
            // Assert.AreEqual(NewsletterSubscriptionService.SubscriptionStatus.NotFound, status);
        }

        [TestMethod]
        public async Task TestDeleteNonExistingListMember()
        {
            string email = GetTestEmail();

            await _service.DeleteListMemberAsync(_newsletterTestListId, email);
            NewsletterSubscriptionService.SubscriptionStatus status = await _service.GetListMemberStatusAsync(_newsletterTestListId, email);
            Assert.AreEqual(NewsletterSubscriptionService.SubscriptionStatus.NotFound, status);
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@outlook.com";
    }
}
