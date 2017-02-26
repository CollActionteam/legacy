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
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Filepath { get; set; }

        [Required]
        [MaxLength(16)]
        [RegularExpression("^(?i:)(?:jpg|jpeg|gif|png|bmp)$", ErrorMessage = "Only JPEG, GIF, PNG and BMP image formats are accepted.")]
        public string Format { get; set; }
        
        [Required]
        public int Width { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
