namespace CollAction.Services.Donation
{
    public class StripeSignatures
    {
        public string StripeChargeableWebhookSecret { get; set; }

        public string StripePaymentEventWebhookSecret { get; set; }
    }
}