using CollAction.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.Services.Projects.Models
{
    public sealed class ProjectResult
    {
        public ProjectResult(params ValidationResult[] errors)
        {
            Errors = errors;
        }

        public ProjectResult(IEnumerable<ValidationResult> errors)
        {
            Errors = errors;
        }

        public ProjectResult(Project project)
        {
            Project = project;
            Succeeded = true;
        }

        public ProjectResult()
        {
            Succeeded = true;
        }

        public Project? Project { get; set; }

        public bool Succeeded { get; set; } = false;

        public IEnumerable<ValidationResult> Errors { get; set; } = Enumerable.Empty<ValidationResult>();
    }
}
