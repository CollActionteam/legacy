using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Email
{
    public sealed class AuthMessageSenderOptions
    {
        [Required]
        [EmailAddress]
        public string FromAddress { get; set; } = null!;

        [Required]
        public string SesAwsAccessKeyID { get; set; } = null!;

        [Required]
        public string SesAwsAccessKey { get; set; } = null!;

        [Required]
        public string SesRegion { get; set; } = null!;
    }
}