using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class ImageFile
    {
        public ImageFile(string filepath, int width, int height, DateTime date, string description)
        {
            Filepath = filepath;
            Width = width;
            Height = height;
            Date = date;
            Description = description;
        }

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
