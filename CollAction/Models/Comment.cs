using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid ExternalReference { get; set; }

        [Required]
        [ForeignKey("ProjectId")]
        public Project Project { get; set;}

        [Required]
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Text { get; set; }

        public List<CommentReply> Replies {get; set;}

        public List<CommentLike> Likes {get; set;}
    }
}