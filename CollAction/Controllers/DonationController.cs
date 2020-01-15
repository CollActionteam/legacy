using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CollAction.Services.Donation;
using Microsoft.AspNetCore.Mvc;

namespace CollAction.Controllers
{
    public class DonationController : Controller
    {
        private readonly IDonationService donationService;

        public DonationController(IDonationService donationService)
        {
            this.donationService = donationService;
        }

        [HttpPost]
        public async Task<IActionResult> PaymentEvent(CancellationToken token)
        {
            using var streamReader = new StreamReader(HttpContext.Request.Body);
            string json = await streamReader.ReadToEndAsync();
            string signature = Request.Headers["Stripe-Signature"];
            await donationService.LogPaymentEvent(json, signature, token);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Chargeable()
        {
            using var streamReader = new StreamReader(HttpContext.Request.Body);
            string json = await streamReader.ReadToEndAsync();
            string signature = Request.Headers["Stripe-Signature"];
            donationService.HandleChargeable(json, signature);
            return Ok();
        }
    }
}