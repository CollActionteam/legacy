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
        public async Task<IActionResult> InitializeCreditCardCheckout(string currency, int amount, string name, string email, bool recurring)
        {
            string checkoutId = await _donationService.InitializeCreditCardCheckout(currency, amount, name, email, recurring);
            return Ok(checkoutId);
        }

        [HttpPost]
        public async Task<IActionResult> InitializeIdealCheckout(string sourceId, string name, string email, bool recurring)
        {
            await _donationService.InitializeIdealCheckout(sourceId, name, email, recurring);
            return Ok();
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