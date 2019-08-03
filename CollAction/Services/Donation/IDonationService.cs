using CollAction.Models;
using Stripe;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email, bool recurring);

        Task InitializeIdealCheckout(string sourceId, string name, string email);

        Task InitializeSepaDirect(string sourceId, string name, string email, int amount);

        Task<bool> HasIDealPaymentSucceeded(string sourceId, string clientSecret);

        Task LogPaymentEvent(string json, string signature);

        void HandleChargeable(string json, string signature);

        Task<IEnumerable<Subscription>> GetSubscriptionsFor(ApplicationUser userFor);

        Task CancelSubscription(string subscriptionId, ClaimsPrincipal userFor);
    }
}