using System;
using System.Threading.Tasks;
using Hangfire;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CollAction.Services.Newsletter
{
    public class NewsletterSubscriptionService : INewsletterSubscriptionService
    {
        private readonly string _newsletterListId;
        private readonly IMailChimpManager _mailChimpManager;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<NewsletterSubscriptionService> _logger;

        public NewsletterSubscriptionService(IMailChimpManager mailChimpManager, IOptions<NewsletterSubscriptionServiceOptions> options, ILogger<NewsletterSubscriptionService> logger, IBackgroundJobClient jobClient)
        {
            _newsletterListId = options.Value.MailChimpNewsletterListId;
            _mailChimpManager = mailChimpManager;
            _jobClient = jobClient;
            _logger = logger;
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
            => _jobClient.Enqueue(() => SetSubscription(email, wantsSubscription, requireEmailConfirmationIfSubscribing));

        public Task SetSubscription(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing)
        {
            _logger.LogInformation("changed maillist subscription for {0} setting it to {1} with require email confirmation to {2}", email, wantsSubscription, requireEmailConfirmationIfSubscribing);
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
                Member member = await _mailChimpManager.Members.GetAsync(_newsletterListId, email);
                member.Status = Status.Subscribed;
                await _mailChimpManager.Members.AddOrUpdateAsync(_newsletterListId, member);
            }
            catch (MailChimpNotFoundException) // New member
            {
                try
                {
                    await _mailChimpManager.Members.AddOrUpdateAsync(_newsletterListId, new Member()
                    {
                        EmailAddress = email,
                        StatusIfNew = usePendingStatusIfNew ? Status.Pending : Status.Subscribed,
                        Status = Status.Subscribed
                    });
                }
                catch (MailChimpException e)
                {
                    if (e.Status == 400 && e.Title.Equals("Forgotten Email Not Subscribed", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new NeedsToResubscribeException(e);
                    }

                    throw;
                }
            }
        }

        public async Task UnsubscribeMember(string email)
        {
            try
            {
                Member member = await _mailChimpManager.Members.GetAsync(_newsletterListId, email);
                member.Status = Status.Unsubscribed;
                await _mailChimpManager.Members.AddOrUpdateAsync(_newsletterListId, member);
            }
            catch(MailChimpNotFoundException)
            {
                // Doesn't exist, so already unsubscribed
            }
        }

        public async Task<Status> GetListMemberStatus(string email)
            => (await _mailChimpManager.Members.GetAsync(_newsletterListId, email)).Status;
    }
}