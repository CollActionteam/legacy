using System;
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

        private Tag? tag;
        [ForeignKey("TagId")]
        public Tag Tag
        {
            get => tag ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Tag)}");
            set => tag = value;
        }

        [Required]
        public int ProjectId { get; set; }

        private Project? project;
        [ForeignKey("ProjectId")]
        public Project Project
        {
            get => project ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Project)}");
            set => project = value;
        }
    }
}
