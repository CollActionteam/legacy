using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public class ImageFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Filepath { get; set; }

        [Required]
        public int Width { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
