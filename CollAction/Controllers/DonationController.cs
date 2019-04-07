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
        public async Task<IActionResult> InitializeCreditCardCheckout(string currency, int amount)
        {
            string checkoutId = await _donationService.InitializeCreditCardCheckout(currency, amount, await _userManager.GetUserAsync(User));
            return Ok(checkoutId);
        }

        [HttpPost]
        public async Task<IActionResult> InitializeIdealCheckout(string sourceId)
        {
            await _donationService.InitializeIdealCheckout(sourceId, await _userManager.GetUserAsync(User));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PaymentEvent([FromBody] JObject stripeEvent)
        {
            await _donationService.LogExternalEvent(stripeEvent);
            return Ok();
        }

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