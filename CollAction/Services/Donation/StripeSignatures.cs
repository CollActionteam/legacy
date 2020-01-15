using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Donation
{
    public class StripeSignatures
    {
        [Required]
        public string StripeChargeableWebhookSecret { get; set; } = null!;

        [Required]
        public string StripePaymentEventWebhookSecret { get; set; } = null!;
    }
}