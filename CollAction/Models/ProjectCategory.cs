using Microsoft.EntityFrameworkCore;

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

        public Project Project { get; set; }

        public Category Category { get; set; }
    }
}
