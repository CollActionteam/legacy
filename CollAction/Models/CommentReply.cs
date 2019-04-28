using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
  public class CommentReply
  {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid ExternalReference { get; set; }

        [Required]
        [ForeignKey("CommentId")]
        public Comment Comment { get; set; }

        [Required]
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Text { get; set; }
  }
}