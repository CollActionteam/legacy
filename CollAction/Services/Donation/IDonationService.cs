﻿using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email);
        Task InitializeIdealCheckout(string sourceId, string name, string email);
        Task<bool> HasIdealPaymentSucceeded(string sourceId, string clientSecret);
        Task LogExternalEvent(JObject stripeEvent);
        Task HandleChargeable(JObject stripeEvent);
    }
}