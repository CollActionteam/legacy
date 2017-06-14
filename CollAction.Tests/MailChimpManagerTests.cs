using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CollAction.Helpers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CollAction.Tests
{
    [TestClass]
    public class MailChimpManagerTests
    {
        private readonly string newsletterListId = "8eb5be39e0"; // The ID for our "Test List" list on MailChimp.

        private MailChimpManager BuildMailChimpManager()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets<Startup>().Build();
            MailChimpManager manager = new MailChimpManager(configuration["MailChimpKey"]);
            return manager;
        }

        [TestMethod]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "non-existing-collaction-test-email@outlook.com";

            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.NotFound);
        }

        [TestMethod]
        public async Task TestAddListMemberAsSubscribed()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterListId, email, false);
            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.Subscribed);
        }

        [TestMethod]
        public async Task TestDeleteExistingListMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterListId, email, false);
            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.Subscribed);

            await manager.DeleteListMemberAsync(newsletterListId, email);
            status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.NotFound);
        }

        [TestMethod]
        public async Task TestDeleteNonExistingListMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "non-existing-collaction-test-email@outlook.com";

            await manager.DeleteListMemberAsync(newsletterListId, email);
            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.NotFound);
        }
    }
}