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
        Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email, bool recurring, CancellationToken cancellationToken);

        Task InitializeIdealCheckout(string sourceId, string name, string email, CancellationToken cancellationToken);

        Task InitializeSepaDirect(string sourceId, string name, string email, int amount, CancellationToken cancellationToken);

        Task<bool> HasIDealPaymentSucceeded(string sourceId, string clientSecret, CancellationToken cancellationToken);

        Task LogPaymentEvent(string json, string signature, CancellationToken cancellationToken);

        void HandleChargeable(string json, string signature);

        Task<IEnumerable<Subscription>> GetSubscriptionsFor(ApplicationUser userFor, CancellationToken cancellationToken);

        Task CancelSubscription(string subscriptionId, ClaimsPrincipal userFor, CancellationToken cancellationToken);
    }
}