using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class UserEvent
    {
        public UserEvent(string eventData, DateTime eventLoggedAt, string? userId)
        {
            EventData = eventData;
            EventLoggedAt = eventLoggedAt;
            UserId = userId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime EventLoggedAt { get; set; }

        [Column(TypeName = "json")]
        [Required]
        public string EventData { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
