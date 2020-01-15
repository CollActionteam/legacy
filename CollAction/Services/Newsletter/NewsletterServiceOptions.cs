using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Newsletter
{
    public class NewsletterServiceOptions
    {
        [Required]
        public string MailChimpNewsletterListId { get; set; } = null!;
    }
}
