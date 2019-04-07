using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(250)]
        public string FirstName { get; set; }

        [MaxLength(250)]
        public string LastName { get; set; }

        public int RepresentsNumberParticipants { get; set; } // Users might represent a business or a school. In that case, one user might represent multiple participants. Only settable by an admin user. Defaults to 1.

        public List<Project> Projects { get; set; }

        public List<ProjectParticipant> Participates { get; set; }

        public List<DonationEventLog> DonationEvents { get; set; }
    }
}
