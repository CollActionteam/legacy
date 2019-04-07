using System.Threading.Tasks;
using CollAction.Models;
using CollAction.Services.Donation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Donate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InitializeCreditCardCheckout(string currency, int amount)
        {
            string checkoutId = await _donationService.InitializeCreditCardCheckout(currency, amount, await _userManager.GetUserAsync(User));
            return Ok(checkoutId);
        }

        [HttpPost]
        public async Task<IActionResult> GetOrCreateCustomer()
        {
            string customerId = (await _donationService.GetOrCreateCustomer(await _userManager.GetUserAsync(User)))?.Id;
            return Ok(customerId);
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}