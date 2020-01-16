using System;

namespace CollAction.Models
{
    public sealed class ProjectCategory
    {
        public ProjectCategory(Category category): this(0, category)
        {
        }

        public ProjectCategory(int projectId, Category category)
        {
            ProjectId = projectId;
            Category = category;
        }

        public int ProjectId { get; set; }

        public Category Category { get; set; }

        private Project? project;
        public Project Project
        {
            get => project ?? throw new InvalidOperationException($"Uninitialized navigation property: {nameof(Project)}");
            set => project = value;
        }
    }
}
