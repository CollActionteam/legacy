using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! 🙂")]
        public bool NewsletterSubscription { get; set; }
        public string Username { get; internal set; }
        public string Email { get; internal set; }
        public bool IsEmailConfirmed { get; internal set; }
        public object StatusMessage { get; internal set; }
    }
}
