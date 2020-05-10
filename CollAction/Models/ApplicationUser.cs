using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public ApplicationUser(string userName, string email, bool emailConfirmed, string? firstName, string? lastName, DateTime registrationDate)
        {
            UserName = userName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            FirstName = firstName;
            LastName = lastName;
            RegistrationDate = registrationDate;
        }

        public ApplicationUser(string email, DateTime registrationDate) : this(email, email, false, null, null, registrationDate)
        {
        }

        public ApplicationUser(string email, string? firstName, string? lastName, DateTime registrationDate) : this(email, email, false, firstName, lastName, registrationDate)
        {
        }

        [MaxLength(250)]
        public string? FirstName { get; set; }

        [MaxLength(250)]
        public string? LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        [NotMapped]
        public string? FullName
        {
            get
            {
                if (FirstName == null)
                {
                    return LastName;
                }
                else if (LastName == null)
                {
                    return FirstName;
                }
                else
                {
                    return $"{FirstName} {LastName}".Trim();
                }
            }
        }

        [NotMapped]
        public bool Activated
            => PasswordHash != null;

        public int RepresentsNumberParticipants { get; set; } = 1; // Users might represent a business or a school. In that case, one user might represent multiple participants. Only settable by an admin user. Defaults to 1.

        public ICollection<Crowdaction> Crowdactions { get; set; } = new List<Crowdaction>();

        public ICollection<CrowdactionParticipant> Participates { get; set; } = new List<CrowdactionParticipant>();

        public ICollection<DonationEventLog> DonationEvents { get; set; } = new List<DonationEventLog>();

        public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}
