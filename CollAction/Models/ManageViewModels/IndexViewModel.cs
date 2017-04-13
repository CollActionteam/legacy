using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        [Display(Name = "I would like to receive an update from CollAction every once in a while - don't worry, we like spam as little as you do! \U0001F642")]
        public bool NewsletterSubscription { get; set; }

        public IList<Project> ProjectsCreated { get; set; }

        public IList<Project> ProjectsParticipated { get; set; }
    }
}
