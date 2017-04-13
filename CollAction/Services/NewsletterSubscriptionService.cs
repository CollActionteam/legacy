using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CollAction.Helpers;

namespace CollAction.Services
{
    public class NewsletterSubscriptionService : INewsletterSubscriptionService
    {
        private static MailChimpManager manager;
        private static readonly string newsletterListId = "2141ab4819";

        public NewsletterSubscriptionService(IOptions<NewsletterSubscriptionServiceOptions> options)
        {
            manager = new MailChimpManager(options.Value.MailChimpKey);
        }

        public async Task<bool> IsSubscribedAsync(string email)
        {
            String status = await manager.GetListMemberStatusAsync(newsletterListId, email);
            return status != null && (status == "pending" || status == "subscribed");
        }

        public async Task SetSubscriptionAsync(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing)
        {
            if (wantsSubscription)
            {
                await manager.AddOrUpdateListMemberAsync(newsletterListId, email, requireEmailConfirmationIfSubscribing);
            }
            else
            {
                await manager.DeleteListMemberAsync(newsletterListId, email);
            }
        }
    }
}