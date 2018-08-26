using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CollAction.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace CollAction.Tests
{
    [TestClass]
    public class NewsletterSubscriptionServiceTests
    {
        private string newsletterTestListId = ""; // The ID for our "Test List" list on MailChimp.

        private NewsletterSubscriptionService BuildNewsletterSubscriptionService()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets<Startup>().Build();
            var options = new NewsletterSubscriptionServiceOptions();
            configuration.Bind(options);
            NewsletterSubscriptionService manager = new NewsletterSubscriptionService(new OptionsWrapper<NewsletterSubscriptionServiceOptions>(options), new LoggerFactory().CreateLogger<NewsletterSubscriptionService>());
            return manager;
        }

        [TestMethod]
        public async Task TestGetListMemberStatusOnNonExistentMember()
        {
            NewsletterSubscriptionService manager = BuildNewsletterSubscriptionService();
            string email = "non-existing-collaction-test-email@outlook.com";

            NewsletterSubscriptionService.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == NewsletterSubscriptionService.SubscriptionStatus.NotFound);
        }

        [TestMethod]
        public async Task TestAddListMemberAsSubscribed()
        {
            NewsletterSubscriptionService manager = BuildNewsletterSubscriptionService();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterTestListId, email, false);
            NewsletterSubscriptionService.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == NewsletterSubscriptionService.SubscriptionStatus.Subscribed);
        }

        [TestMethod]
        public async Task TestDeleteExistingListMember()
        {
            NewsletterSubscriptionService manager = BuildNewsletterSubscriptionService();
            string email = "collaction-test-email@outlook.com";

            await manager.AddOrUpdateListMemberAsync(newsletterTestListId, email, false);
            NewsletterSubscriptionService.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == NewsletterSubscriptionService.SubscriptionStatus.Subscribed);

            await manager.DeleteListMemberAsync(newsletterTestListId, email);
            status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == NewsletterSubscriptionService.SubscriptionStatus.NotFound);
        }

        [TestMethod]
        public async Task TestDeleteNonExistingListMember()
        {
            NewsletterSubscriptionService manager = BuildNewsletterSubscriptionService();
            string email = "non-existing-collaction-test-email@outlook.com";

            await manager.DeleteListMemberAsync(newsletterTestListId, email);
            NewsletterSubscriptionService.SubscriptionStatus status = await manager.GetListMemberStatusAsync(newsletterTestListId, email);
            Assert.IsTrue(status == NewsletterSubscriptionService.SubscriptionStatus.NotFound);
        }
    }
}
