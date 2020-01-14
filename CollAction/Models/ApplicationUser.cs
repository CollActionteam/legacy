using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(250)]
        public string? FirstName { get; set; }

        [MaxLength(250)]
        public string? LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        [NotMapped]
        public string FullName
            => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public bool Activated
            => PasswordHash != null;

        public int RepresentsNumberParticipants { get; set; } // Users might represent a business or a school. In that case, one user might represent multiple participants. Only settable by an admin user. Defaults to 1.

        public ICollection<Project> Projects { get; set; }

        public ICollection<ProjectParticipant> Participates { get; set; }

        public ICollection<DonationEventLog> DonationEvents { get; set; }

        public ICollection<UserEvent> UserEvents { get; set; }
    }
}
