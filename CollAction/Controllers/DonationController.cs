using System.Threading.Tasks;
using CollAction.Models;
using CollAction.Services.Donation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CollAction.Controllers
{
    public class DonationController : Controller
    {
        private IDonationService _donationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonationController(IDonationService donationService, UserManager<ApplicationUser> userManager)
        {
            _donationService = donationService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> InitializeCreditCardCheckout(string currency, int amount, string name, string email)
        {
            string checkoutId = await _donationService.InitializeCreditCardCheckout(currency, amount, name, email);
            return Ok(checkoutId);
        }

        [HttpPost]
        public async Task<IActionResult> InitializeIdealCheckout(string sourceId, string name, string email)
        {
            await _donationService.InitializeIdealCheckout(sourceId, name, email);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PaymentEvent([FromBody] JObject stripeEvent)
        {
            await _donationService.LogExternalEvent(stripeEvent);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Return(string source, string client_secret, bool livemode)
        {
            if (await _donationService.HasIdealPaymentSucceeded(source, client_secret))
            {
                return RedirectToAction(nameof(ThankYou));
            }
            else
            {
                return RedirectToAction(nameof(Donate));
            }
        }

        [HttpGet]
        public IActionResult Donate()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}