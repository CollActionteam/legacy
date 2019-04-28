using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
  public class CommentLike
  {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("CommentId")]
        public Comment Comment { get; set; }

        [Required]
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }
  }
}