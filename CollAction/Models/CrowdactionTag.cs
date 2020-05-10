using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class CrowdactionTag
    {
        public CrowdactionTag(int tagId, int crowdactionId)
        {
            TagId = tagId;
            CrowdactionId = crowdactionId;
        }

        public CrowdactionTag(int tagId) : this(tagId, 0)
        {
        }

        [Required]
        public int TagId { get; set; }

        [ForeignKey("TagId")]
        public Tag? Tag { get; set; }

        [Required]
        public int CrowdactionId { get; set; }

        [ForeignKey("CrowdactionId")]
        public Crowdaction? Crowdaction { get; set; }
    }
}
