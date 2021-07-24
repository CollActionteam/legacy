using System;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class CrowdactionComment
    {
        public CrowdactionComment(string comment, string? userId, string? anonymousCommentUser, int crowdactionId, DateTime commentedAt, CrowdactionCommentStatus status)
        {
            Comment = comment;
            UserId = userId;
            AnonymousCommentUser = anonymousCommentUser;
            CrowdactionId = crowdactionId;
            CommentedAt = commentedAt;
            Status = status;
        }

        public int Id { get; set; }

        public DateTime CommentedAt { get; set; }

        [Required]
        public string Comment { get; set; } = null!;

        [MaxLength(20)]
        public string? AnonymousCommentUser { get; set; } = null!;

        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }

        public int CrowdactionId { get; set; }

        public Crowdaction? Crowdaction { get; set; }

        public CrowdactionCommentStatus Status { get; set; }
    }
}
