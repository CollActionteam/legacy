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
        public int Color { get; set; }

        [NotMapped]
        public string ColorHex
        {
            get
            {
                return unchecked((uint)Color).ToString("X6");
            }

            set
            {
                Color = unchecked((int)uint.Parse(value, NumberStyles.HexNumber));
            }
        }
    }
}
