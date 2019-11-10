using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public class ProjectParticipant
    {
        public ProjectParticipant()
        {
            ParticipationDate = DateTime.UtcNow;
        }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public bool SubscribedToProjectEmails { get; set; }

        public DateTime ParticipationDate { get; set; }

        public Guid UnsubscribeToken { get; set; }
    }
}
