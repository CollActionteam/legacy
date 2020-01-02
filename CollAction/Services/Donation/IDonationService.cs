using CollAction.Models;
using Stripe;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email, bool recurring, CancellationToken token);

        Task InitializeIdealCheckout(string sourceId, string name, string email, CancellationToken token);

        Task InitializeSepaDirect(string sourceId, string name, string email, int amount, CancellationToken token);

        Task<bool> HasIDealPaymentSucceeded(string sourceId, string clientSecret, CancellationToken token);

        Task LogPaymentEvent(string json, string signature, CancellationToken token);

        void HandleChargeable(string json, string signature);

        Task<IEnumerable<Subscription>> GetSubscriptionsFor(ApplicationUser userFor, CancellationToken token);

        Task CancelSubscription(string subscriptionId, ClaimsPrincipal userFor, CancellationToken token);
    }
}