using CollAction.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.Projects.Models
{
    public sealed class ProjectResult
    {
        public Project Project { get; set; }

        public bool Succeeded { get; set; }

        public IEnumerable<ValidationResult> Errors { get; set; }
    }
}
