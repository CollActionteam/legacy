using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Donation
{
    public sealed class StripePublicOptions
    {
        [Required]
        public string StripePublicKey { get; set; } = null!;
    }
}
