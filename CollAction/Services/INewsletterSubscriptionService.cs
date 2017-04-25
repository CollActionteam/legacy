using System;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public interface INewsletterSubscriptionService
    {
        Task SetSubscriptionAsync(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing = true);
        Task<bool> IsSubscribedAsync(string email);
    }
}
