using MailChimp.Net.Models;
using System.Threading.Tasks;

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
