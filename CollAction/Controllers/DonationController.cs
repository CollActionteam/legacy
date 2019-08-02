using System.IO;
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

        [HttpPost]
        public async Task<IActionResult> PaymentEvent()
        {
            using (var streamReader = new StreamReader(HttpContext.Request.Body))
            {
                string json = await streamReader.ReadToEndAsync();
                string signature = Request.Headers["Stripe-Signature"];
                await _donationService.LogPaymentEvent(json, signature);
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Chargeable()
        {
            using (var streamReader = new StreamReader(HttpContext.Request.Body))
            {
                string json = await streamReader.ReadToEndAsync();
                string signature = Request.Headers["Stripe-Signature"];
                _donationService.HandleChargeable(json, signature);
                return Ok();
            }
        }
    }
}