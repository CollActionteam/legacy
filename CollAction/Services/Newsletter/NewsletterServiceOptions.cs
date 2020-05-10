using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Newsletter
{
    public sealed class NewsletterServiceOptions
    {
        [Required]
        public string MailChimpNewsletterListId { get; set; } = null!;

        [Required]
        public string MailChimpUserId { get; set; } = null!;

        [Required]
        public string MailChimpServer { get; set; } = null!;

        [Required]
        public string MailChimpAccount { get; set; } = null!;
    }
}
