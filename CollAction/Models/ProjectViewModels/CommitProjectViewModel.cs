using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class CommitProjectViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public bool IsUserCommitted { get; set; } = false;
    }
}
