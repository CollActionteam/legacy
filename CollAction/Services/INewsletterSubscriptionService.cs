using System;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public interface INewsletterSubscriptionService
    {
        void SetSubscription(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing = true);
        Task<bool> IsSubscribedAsync(string email);
    }
}
