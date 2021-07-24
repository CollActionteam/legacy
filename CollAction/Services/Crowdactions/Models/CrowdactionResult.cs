using CollAction.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.Services.Crowdactions.Models
{
    public sealed class CrowdactionResult
    {
        public CrowdactionResult(params ValidationResult[] errors)
        {
            Errors = errors;
        }

        public CrowdactionResult(IEnumerable<ValidationResult> errors)
        {
            Errors = errors;
        }

        public CrowdactionResult(Crowdaction crowdaction)
        {
            Crowdaction = crowdaction;
            Succeeded = true;
        }

        public CrowdactionResult()
        {
            Succeeded = true;
        }

        public Crowdaction? Crowdaction { get; set; }

        public bool Succeeded { get; set; }

        public IEnumerable<ValidationResult> Errors { get; set; } = Enumerable.Empty<ValidationResult>();
    }
}
