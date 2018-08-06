using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CollAction.Services.Newsletter
{
    public class NewsletterSubscriptionService : INewsletterSubscriptionService
    {
        private readonly MailChimpManager _manager;
        private readonly string _newsletterListId;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<NewsletterSubscriptionService> _logger;

        public NewsletterSubscriptionService(IOptions<NewsletterSubscriptionServiceOptions> options, IBackgroundJobClient jobClient, ILogger<NewsletterSubscriptionService> logger)
        {
            _manager = new MailChimpManager(options.Value.MailChimpKey);
            _newsletterListId = options.Value.MailChimpNewsletterListId;
            _jobClient = jobClient;
            _logger = logger;
        }

        public async Task<bool> IsSubscribedAsync(string email)
        {
            MailChimpManager.SubscriptionStatus status = await _manager.GetListMemberStatusAsync(_newsletterListId, email);
            return status == MailChimpManager.SubscriptionStatus.Pending || status == MailChimpManager.SubscriptionStatus.Subscribed;
        }

        public void SetSubscription(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing)
        {
            string job = wantsSubscription ? _jobClient.Enqueue(() => _manager.AddOrUpdateListMemberAsync(_newsletterListId, email, requireEmailConfirmationIfSubscribing)) :
                                             _jobClient.Enqueue(() => _manager.DeleteListMemberAsync(_newsletterListId, email));
            _logger.LogInformation("changed maillist subscription for {0} setting it to {1} with require email confirmation to {2} and hangfire job {3}", email, wantsSubscription, requireEmailConfirmationIfSubscribing, job);
        }
    }
}