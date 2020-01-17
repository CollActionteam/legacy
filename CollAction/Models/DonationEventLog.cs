using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class DonationEventLog
    {
        public DonationEventLog(string eventData, DonationEventType type, string? userId = null)
        {
            EventData = eventData;
            UserId = userId;
            Type = type;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "json")]
        public string EventData { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public DonationEventType Type { get; set; }
    }
}
