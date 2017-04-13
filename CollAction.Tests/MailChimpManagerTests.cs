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
        private readonly string newsletterListId = "2141ab4819"; // The ID for our "CollAction Newsletter" list on MailChimp.

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

            String status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsNull(status);
        }

        [TestMethod]
        public async Task TestAddListMemberAsSubscribed()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterListId, email, false);
            String status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsNotNull(status);
            Assert.IsTrue(status == "subscribed");
        }

        [TestMethod]
        public async Task TestDeleteExistingListMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterListId, email, false);
            String status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsNotNull(status);
            Assert.IsTrue(status == "subscribed");

            await manager.DeleteListMemberAsync(newsletterListId, email);
            status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsNull(status);
        }

        [TestMethod]
        public async Task TestDeleteNonExistingListMember()
        {
            MailChimpManager manager = BuildMailChimpManager();
            string email = "non-existing-collaction-test-email@outlook.com";

            await manager.DeleteListMemberAsync(newsletterListId, email);
            String status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            Assert.IsNull(status);
        }
    }
}