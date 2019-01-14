using System.Threading.Tasks;
using CollAction.Services.Donation;
using Microsoft.AspNetCore.Mvc;

namespace CollAction.Controllers
{
    public class DonationController : Controller
    {
        private IDonationService _donationService;

        public DonationController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        public IActionResult Donation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PerformDonation(string email, int amount, string currency, string token) 
        {
            await _donationService.Charge(email, token, amount, currency);
            return View();
        }
    }
}