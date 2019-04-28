﻿using System.Threading.Tasks;

namespace CollAction.Services.Donation
{
    public interface IDonationService
    {
        Task<string> InitializeCreditCardCheckout(string currency, int amount, string name, string email);
        Task InitializeIdealCheckout(string sourceId, string name, string email);
        Task<bool> HasIdealPaymentSucceeded(string sourceId, string clientSecret);
        Task LogPaymentEvent(string json, string signature);
        void HandleChargeable(string json, string signature);
    }
}