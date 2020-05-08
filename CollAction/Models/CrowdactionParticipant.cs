using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class CrowdactionParticipant
    {
        public CrowdactionParticipant(string userId, int crowdactionId, bool subscribedToCrowdactionEmails, DateTime participationDate, Guid unsubscribeToken)
        {
            UserId = userId;
            CrowdactionId = crowdactionId;
            SubscribedToCrowdactionEmails = subscribedToCrowdactionEmails;
            ParticipationDate = participationDate;
            UnsubscribeToken = unsubscribeToken;
        }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public int CrowdactionId { get; set; }

        [ForeignKey("CrowdactionId")]
        public Crowdaction? Crowdaction { get; set; }

        public bool SubscribedToCrowdactionEmails { get; set; }

        public DateTime ParticipationDate { get; set; }

        public Guid UnsubscribeToken { get; set; }
    }
}
