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

        private ApplicationUser? user;
        [ForeignKey("UserId")]
        public ApplicationUser User
        {
            get => user ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(User)}");
            set => user = value;
        }

        [Required]
        public int CrowdactionId { get; set; }

        private Crowdaction? crowdaction;
        [ForeignKey("CrowdactionId")]
        public Crowdaction Crowdaction
        {
            get => crowdaction ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Crowdaction)}");
            set => crowdaction = value;
        }

        public bool SubscribedToCrowdactionEmails { get; set; }

        public DateTime ParticipationDate { get; set; }

        public Guid UnsubscribeToken { get; set; }
    }
}
