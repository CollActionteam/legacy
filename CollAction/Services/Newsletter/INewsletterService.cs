using System.Threading.Tasks;
using MailChimp.Net.Models;

namespace CollAction.Services.Newsletter
{
    public interface INewsletterService
    {
        Task SetSubscription(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing = true);

        void SetSubscriptionBackground(string email, bool wantsSubscription, bool requireEmailConfirmationIfSubscribing = true);

        Task<bool> IsSubscribedAsync(string email);

        Task<Status> GetListMemberStatus(string email);

        Task UnsubscribeMember(string email);
    }
}
