using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollAction.Models
{
    public sealed class ProjectTag
    {
        public ProjectTag(int tagId, int projectId)
        {
            TagId = tagId;
            ProjectId = projectId;
        }

        public ProjectTag(int tagId): this(tagId, 0)
        {
        }

        [Required]
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public Tag Tag { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}
