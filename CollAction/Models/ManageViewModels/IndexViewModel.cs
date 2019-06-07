using Stripe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! 🙂")]
        public bool NewsletterSubscription { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IEnumerable<Subscription> DonationSubscriptions { get; set; }
    }
}
