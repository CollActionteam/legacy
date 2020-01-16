using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class ProjectParticipant
    {
        public ProjectParticipant(string userId, int projectId, bool subscribedToProjectEmails, DateTime participationDate, Guid unsubscribeToken)
        {
            UserId = userId;
            ProjectId = projectId;
            SubscribedToProjectEmails = subscribedToProjectEmails;
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
        public int ProjectId { get; set; }

        private Project? project;
        [ForeignKey("ProjectId")]
        public Project Project
        {
            get => project ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Project)}");
            set => project = value;
        }

        public bool SubscribedToProjectEmails { get; set; }

        public DateTime ParticipationDate { get; set; }

        public Guid UnsubscribeToken { get; set; }
    }
}
