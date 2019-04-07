using CollAction.Models;
using Stripe;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task<string> InitializeCreditCardCheckout(string currency, int amount, ApplicationUser user);
        Task<Customer> GetOrCreateCustomer(ApplicationUser applicationUser);
    }
}