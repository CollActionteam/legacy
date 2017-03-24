using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CollAction.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CollAction.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(250)]
        public string FirstName { get; set; }

        [MaxLength(250)]
        public string LastName { get; set; }

        public List<Project> Projects { get; set; }

        public List<ProjectParticipant> Participates { get; set; }

        public int? NewsletterSubscriptionId { get; set; }
        [ForeignKey("NewsletterSubscriptionId")]
        public NewsletterSubscription NewsletterSubscription { get; set; }

        public async Task SetNewsletterSubscription(ApplicationDbContext context, string email, bool wantsSubscription)
        {
            NewsletterSubscription subscription = await context.NewsletterSubscriptions.SingleOrDefaultAsync(s => s.Email.Equals(email, StringComparison.Ordinal));
            if (wantsSubscription)
            {
                NewsletterSubscription = subscription == null ? new NewsletterSubscription { Email = email } : subscription;
            }
            else
            {
                if (subscription != null)
                {
                    context.NewsletterSubscriptions.Remove(subscription);
                    await context.SaveChangesAsync();
                }
                NewsletterSubscription = null;
            }
        }
    }
}
