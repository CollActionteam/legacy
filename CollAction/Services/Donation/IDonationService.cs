using CollAction.Models;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task<string> InitializeCreditCardCheckout(string currency, int amount, ApplicationUser user);
        Task InitializeIdealCheckout(string sourceId, ApplicationUser user);
        Task LogExternalEvent(JObject stripeEvent);
    }
}