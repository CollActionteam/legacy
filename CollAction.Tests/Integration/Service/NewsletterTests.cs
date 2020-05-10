using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CollAction.Services.Newsletter;
using System;
using Moq;
using Hangfire;
using MailChimp.Net.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.Tests.Integration.Service
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class NewsletterServiceTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestGetListMemberStatusOnNonExistentMember()
            => WithServiceProvider(
                async scope =>
                {
                    var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                    string email = GetTestEmail();
                    Assert.IsFalse(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
                });

        [TestMethod]
        public Task TestSubscribeListMemberAsPending()
            => WithServiceProvider(
                   async scope =>
                   {
                       var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                       string email = GetTestEmail();

                       try
                       {
                           await newsletterService.SetSubscription(email, true, true).ConfigureAwait(false);
                           Status status = await newsletterService.GetListMemberStatus(email).ConfigureAwait(false);
                           Assert.AreEqual(Status.Pending, status);
                           Assert.IsTrue(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
                       }
                       finally
                       {
                           await newsletterService.UnsubscribeMember(email).ConfigureAwait(false);
                       }
                   });

        [TestMethod]
        public Task TestSubscribeListMemberAsSubscribed()
            => WithServiceProvider(
                   async scope =>
                   {
                       var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                       string email = GetTestEmail();

                       try
                       {
                           await newsletterService.SetSubscription(email, true, false).ConfigureAwait(false);
                           Status status = await newsletterService.GetListMemberStatus(email).ConfigureAwait(false);
                           Assert.AreEqual(Status.Subscribed, status);
                           Assert.IsTrue(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
                       }
                       finally
                       {
                           await newsletterService.UnsubscribeMember(email).ConfigureAwait(false);
                       }
                   });

        [TestMethod]
        public Task TestUnsubscribeExistingListMember()
            => WithServiceProvider(
                   async scope =>
                   {
                       var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                       string email = GetTestEmail();

                       try
                       {
                           await newsletterService.SetSubscription(email, true, true).ConfigureAwait(false);
                           Status status = await newsletterService.GetListMemberStatus(email).ConfigureAwait(false);
                           Assert.AreEqual(Status.Pending, status);
   
                           await newsletterService.SetSubscription(email, false, false).ConfigureAwait(false);
                           Assert.IsFalse(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
                       }
                       finally
                       {
                           await newsletterService.SetSubscription(email, false, false).ConfigureAwait(false);
                       }
                   });

        [TestMethod]
        public Task TestUnsubscribeSubscribeMultiple()
            => WithServiceProvider(
                   async scope =>
                   {
                       var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                       string email = GetTestEmail();

                       try
                       {
                           for (int attempt = 0; attempt < 4; attempt++)
                           {
                               for (bool requireEmail = true; requireEmail; requireEmail = !requireEmail)
                               {
                                   await newsletterService.SetSubscription(email, true, requireEmail).ConfigureAwait(false);
                                   Assert.IsTrue(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));

                                   await newsletterService.SetSubscription(email, false, requireEmail).ConfigureAwait(false);
                                   Assert.IsFalse(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
                               }
                           }
                       }
                       finally
                       {
                           await newsletterService.SetSubscription(email, false, true).ConfigureAwait(false);
                       }
                   });

        [TestMethod]
        public Task TestUnsubscribeNonExistingListMember()
            => WithServiceProvider(
                   async scope =>
                   {
                       var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                       string email = GetTestEmail();

                       await newsletterService.SetSubscription(email, false, false).ConfigureAwait(false);
                       Assert.IsFalse(await newsletterService.IsSubscribedAsync(email).ConfigureAwait(false));
                   });

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            var jobClient = new Mock<IBackgroundJobClient>();
            collection.AddScoped(s => jobClient.Object);
        }

        private static string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@collaction.org";
    }
}
