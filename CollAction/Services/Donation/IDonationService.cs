using CollAction.Models;
using CollAction.Services.Donation.Models;
using Stripe;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task<string> InitializeCreditCardCheckout(CreditCardCheckout checkout, CancellationToken token);

        Task InitializeIDealCheckout(IDealCheckout checkout, CancellationToken token);

        Task InitializeSepaDirect(SepaDirectCheckout checkout, CancellationToken token);

        Task<bool> HasIDealPaymentSucceeded(string sourceId, string clientSecret, CancellationToken token);

        Task LogPaymentEvent(string json, string signature, CancellationToken token);

        void HandleChargeable(string json, string signature);

        Task<IEnumerable<Subscription>> GetSubscriptionsFor(ApplicationUser userFor, CancellationToken token);

        Task CancelSubscription(string subscriptionId, ClaimsPrincipal userFor, CancellationToken token);
    }
}