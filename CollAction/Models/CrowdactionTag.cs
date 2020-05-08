using System;
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

        public CrowdactionTag(int tagId): this(tagId, 0)
        {
        }

        [Required]
        public int TagId { get; set; }

        private Tag? tag;
        [ForeignKey("TagId")]
        public Tag Tag
        {
            get => tag ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Tag)}");
            set => tag = value;
        }

        [Required]
        public int CrowdactionId { get; set; }

        private Crowdaction? crowdaction;
        [ForeignKey("CrowdactionId")]
        public Crowdaction Crowdaction
        {
            get => crowdaction ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Crowdaction)}");
            set => crowdaction = value;
        }
    }
}
