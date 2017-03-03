using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public class LocationAlternateName
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        [Required]
        [MaxLength(7)]
        public string LanguageCode { get; set; }

        [Required]
        [MaxLength(400)]
        public string AlternateName { get; set; }

        public bool IsPreferredName { get; set; }
        public bool IsShortName { get; set; }
        public bool IsColloquial { get; set; }
        public bool IsHistoric { get; set; }
    }
}
