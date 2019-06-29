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
                Status status = await GetListMemberStatusAsync(email);
                return status == Status.Pending || status == Status.Subscribed;
            }
            catch (MailChimpNotFoundException)
            {
                return false;
            }
        }

        public void SetSubscription(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing)
        {
            string job = wantsSubscription ? _jobClient.Enqueue(() => AddOrUpdateListMemberAsync(email, requireEmailConfirmationIfSubscribing)) :
                                             _jobClient.Enqueue(() => DeleteListMemberAsync(email));
            _logger.LogInformation("changed maillist subscription for {0} setting it to {1} with require email confirmation to {2} and hangfire job {3}", email, wantsSubscription, requireEmailConfirmationIfSubscribing, job);
        }

        public Task AddOrUpdateListMemberAsync(string email, bool usePendingStatusIfNew = true)
             => _mailChimpManager.Members.AddOrUpdateAsync(_newsletterListId, new Member()
            {
                EmailAddress = email,
                StatusIfNew = usePendingStatusIfNew ? Status.Pending : Status.Subscribed
            });

        public Task DeleteListMemberAsync(string email)
            => _mailChimpManager.Members.PermanentDeleteAsync(_newsletterListId, email);

        public async Task<Status> GetListMemberStatusAsync(string email)
            => (await _mailChimpManager.Members.GetAsync(_newsletterListId, email)).Status;
    }
}