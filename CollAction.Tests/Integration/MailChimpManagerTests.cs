using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CollAction.Services.Newsletter;
using System;

namespace CollAction.Tests.Integration
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class MailChimpManagerTests
    {
        private string _newsletterTestListId;
        private readonly MailChimpManager _manager;

        public MailChimpManagerTests()
        {
            IConfiguration configuration = 
                new ConfigurationBuilder().AddUserSecrets<Startup>()
                                          .AddEnvironmentVariables()
                                          .Build();
            _newsletterTestListId = configuration["MailChimpTestListId"];
            _manager = new MailChimpManager(configuration["MailChimpKey"]);
        }

        [TestMethod]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            string email = GetTestEmail();

            MailChimpManager.SubscriptionStatus status = await _manager.GetListMemberStatusAsync(_newsletterTestListId, email);
            Assert.AreEqual(MailChimpManager.SubscriptionStatus.NotFound, status);
        }

        [TestMethod]
        public async Task TestAddListMemberAsPending()
        {
            string email = GetTestEmail();

            try
            {
                await _manager.AddOrUpdateListMemberAsync(_newsletterTestListId, email, true);
                MailChimpManager.SubscriptionStatus status = await _manager.GetListMemberStatusAsync(_newsletterTestListId, email);
                Assert.AreEqual(MailChimpManager.SubscriptionStatus.Pending, status);
            }
            finally
            {
                await _manager.DeleteListMemberAsync(_newsletterTestListId, email);
            }
        }

        [TestMethod]
        public async Task TestAddListMemberAsSubscribed()
        {
            string email = GetTestEmail();

            try
            {
                await _manager.AddOrUpdateListMemberAsync(_newsletterTestListId, email, false);
                MailChimpManager.SubscriptionStatus status = await _manager.GetListMemberStatusAsync(_newsletterTestListId, email);
                Assert.AreEqual(MailChimpManager.SubscriptionStatus.Subscribed, status);
            }
            finally
            {
                await _manager.DeleteListMemberAsync(_newsletterTestListId, email);
            }
        }

        [TestMethod]
        public async Task TestDeleteExistingListMember()
        {
            string email = GetTestEmail();

            await _manager.AddOrUpdateListMemberAsync(_newsletterTestListId, email);
            MailChimpManager.SubscriptionStatus status = await _manager.GetListMemberStatusAsync(_newsletterTestListId, email);
            Assert.AreEqual(MailChimpManager.SubscriptionStatus.Pending, status);

            await _manager.DeleteListMemberAsync(_newsletterTestListId, email);
            status = await _manager.GetListMemberStatusAsync(_newsletterTestListId, email);
            Assert.AreEqual(MailChimpManager.SubscriptionStatus.NotFound, status);
        }

        [TestMethod]
        public async Task TestDeleteNonExistingListMember()
        {
            string email = GetTestEmail();

            await _manager.DeleteListMemberAsync(_newsletterTestListId, email);
            MailChimpManager.SubscriptionStatus status = await _manager.GetListMemberStatusAsync(_newsletterTestListId, email);
            Assert.AreEqual(MailChimpManager.SubscriptionStatus.NotFound, status);
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@outlook.com";
    }
}
