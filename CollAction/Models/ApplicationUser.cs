using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() 
        {
            RepresentsNumberParticipants = 1;        
        }

        public ApplicationUser(string email) 
            : base()
        {
            UserName = email;
            Email = email;
        }

        [MaxLength(250)]
        public string FirstName { get; set; }

        [MaxLength(250)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName
            => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public bool Activated
            => PasswordHash != null;

        public int RepresentsNumberParticipants { get; set; } // Users might represent a business or a school. In that case, one user might represent multiple participants. Only settable by an admin user. Defaults to 1.

        public List<Project> Projects { get; set; }

        public List<ProjectParticipant> Participates { get; set; }

        public List<DonationEventLog> DonationEvents { get; set; }
    }
}
