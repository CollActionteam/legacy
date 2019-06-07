using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollAction.Models;
using CollAction.Services.Donation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

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
        public async Task<IActionResult> InitializeCreditCardCheckout(string currency, int amount, string name, string email, bool recurring)
        {
            string checkoutId = await _donationService.InitializeCreditCardCheckout(currency, amount, name, email, recurring);
            return Ok(checkoutId);
        }

        [HttpPost]
        public async Task<IActionResult> InitializeSepaDirect(string sourceId, string name, string email, int amount)
        {
            await _donationService.InitializeSepaDirect(sourceId, name, email, amount);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> InitializeIdealCheckout(string sourceId, string name, string email)
        {
            await _donationService.InitializeIdealCheckout(sourceId, name, email);
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CancelSubscription(string subscriptionId)
        {
            await _donationService.CancelSubscription(subscriptionId, await _userManager.GetUserAsync(User));
            return View();
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