using Microsoft.EntityFrameworkCore;

namespace CollAction.Models
{
    public class ProjectCategory
    {
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public Category Category { get; set; }
    }
}
