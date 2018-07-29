using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CollAction.Services;

namespace CollAction.Tests
{
    [TestClass]
    public class MailChimpManagerTests
    {
        private string newsletterTestListId = ""; // The ID for our "Test List" list on MailChimp.

        private MailChimpManager BuildMailChimpManager()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets<Startup>().Build();
            newsletterTestListId = configuration["MailChimpTestListId"];
            MailChimpManager manager = new MailChimpManager(configuration["MailChimpKey"]);
            return manager;
        }

        [TestMethod]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "non-existing-collaction-test-email@outlook.com";

            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.NotFound);
        }

        [TestMethod]
        public async Task TestAddListMemberAsSubscribed()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterTestListId, email, false);
            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.Subscribed);
        }

        [TestMethod]
        public async Task TestDeleteExistingListMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterTestListId, email, false);
            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.Subscribed);

            await manager.DeleteListMemberAsync(newsletterTestListId, email);
            status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.NotFound);
        }

        [TestMethod]
        public async Task TestDeleteNonExistingListMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "non-existing-collaction-test-email@outlook.com";

            await manager.DeleteListMemberAsync(newsletterTestListId, email);
            MailChimpManager.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == MailChimpManager.SubscriptionStatus.NotFound);
        }
    }
}
