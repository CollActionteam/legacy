using System.Threading.Tasks;
using Hangfire;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CollAction.Services.Newsletter
{
    public class NewsletterService : INewsletterService
    {
        private readonly string newsletterListId;
        private readonly IMailChimpManager mailChimpManager;
        private readonly IBackgroundJobClient jobClient;
        private readonly ILogger<NewsletterService> logger;

        public NewsletterService(IMailChimpManager mailChimpManager, IOptions<NewsletterServiceOptions> options, ILogger<NewsletterService> logger, IBackgroundJobClient jobClient)
        {
            newsletterListId = options.Value.MailChimpNewsletterListId;
            this.mailChimpManager = mailChimpManager;
            this.jobClient = jobClient;
            this.logger = logger;
        }

        public async Task<bool> IsSubscribedAsync(string email)
        {
            try
            {
                Status status = await GetListMemberStatus(email);
                return status == Status.Pending || status == Status.Subscribed;
            }
            catch (MailChimpNotFoundException)
            {
                return false;
            }
        }

        public void SetSubscriptionBackground(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing)
            => jobClient.Enqueue(() => SetSubscription(email, wantsSubscription, requireEmailConfirmationIfSubscribing));

        public Task SetSubscription(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing)
        {
            logger.LogInformation("Changing maillist subscription for user, setting it to {1} with require email confirmation to {2}", email, wantsSubscription, requireEmailConfirmationIfSubscribing);
            if (wantsSubscription)
            {
                return SubscribeMember(email, requireEmailConfirmationIfSubscribing);
            }
            else
            {
                 return UnsubscribeMember(email);
            }
        }

        public async Task SubscribeMember(string email, bool usePendingStatusIfNew = true)
        {
            try
            {
                Member member = await mailChimpManager.Members.GetAsync(newsletterListId, email);
                member.Status = Status.Subscribed;
                await mailChimpManager.Members.AddOrUpdateAsync(newsletterListId, member);
            }
            catch (MailChimpNotFoundException)
            { 
                // New member
                await mailChimpManager.Members.AddOrUpdateAsync(
                    newsletterListId, 
                    new Member()
                    {
                        EmailAddress = email,
                        StatusIfNew = usePendingStatusIfNew ? Status.Pending : Status.Subscribed,
                        Status = Status.Subscribed
                    });
            }

            logger.LogInformation("Successfully subscribed to newsletter");
        }

        public async Task UnsubscribeMember(string email)
        {
            try
            {
                Member member = await mailChimpManager.Members.GetAsync(newsletterListId, email);
                member.Status = Status.Unsubscribed;
                await mailChimpManager.Members.AddOrUpdateAsync(newsletterListId, member);
            }
            catch (MailChimpNotFoundException)
            {
                // Doesn't exist, so already unsubscribed
            }

            logger.LogInformation("Successfully unsubscribed from newsletter");
        }

        public async Task<Status> GetListMemberStatus(string email)
            => (await mailChimpManager.Members.GetAsync(newsletterListId, email)).Status;
    }
}