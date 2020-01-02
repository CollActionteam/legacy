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
    [TestCategory("CantRunInTravis")]
    public sealed class NewsletterServiceTests : IntegrationTestBase
    {
        [TestMethod]
        public Task TestGetListMemberStatusOnNonExistentMember()
            => WithServiceProvider(
                async scope =>
                {
                    var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                    string email = GetTestEmail();
                    Assert.IsFalse(await newsletterService.IsSubscribedAsync(email));
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
                           await newsletterService.SetSubscription(email, true, true);
                           Status status = await newsletterService.GetListMemberStatus(email);
                           Assert.AreEqual(Status.Pending, status);
                           Assert.IsTrue(await newsletterService.IsSubscribedAsync(email));
                       }
                       finally
                       {
                           await newsletterService.UnsubscribeMember(email);
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
                           await newsletterService.SetSubscription(email, true, false);
                           Status status = await newsletterService.GetListMemberStatus(email);
                           Assert.AreEqual(Status.Subscribed, status);
                           Assert.IsTrue(await newsletterService.IsSubscribedAsync(email));
                       }
                       finally
                       {
                           await newsletterService.UnsubscribeMember(email);
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
                   });

        [TestMethod]
        public Task TestUnsubscribeNonExistingListMember()
            => WithServiceProvider(
                   async scope =>
                   {
                       var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();
                       string email = GetTestEmail();

                       await newsletterService.SetSubscription(email, false, false);
                       Assert.IsFalse(await newsletterService.IsSubscribedAsync(email));
                   });

        protected override void ConfigureReplacementServicesProvider(IServiceCollection collection)
        {
            var jobClient = new Mock<IBackgroundJobClient>();
            collection.AddScoped(s => jobClient.Object);
        }

        private string GetTestEmail()
            => $"collaction-test-email-{Guid.NewGuid()}@collaction.org";
    }
}
