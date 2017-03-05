using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace CollAction.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        [MaxLength(120)]
        public string Description { get; set; }

        [Required]
        public int Color { get; set; }

        [Required]
        [MaxLength(255)]
        public string File { get; set; }

        [NotMapped]
        public string ColorHex
        {
            get
            {
                return (unchecked((uint)Color)).ToString("X");
            }

            set
            {
                Color = unchecked((int) uint.Parse(value, NumberStyles.HexNumber));
            }
        }
    }
}
