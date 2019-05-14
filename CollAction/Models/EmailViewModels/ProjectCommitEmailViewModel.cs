using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Models.EmailViewModels
{
    public class ProjectCommitEmailViewModel
    {
        public string ProjectUrl { get; internal set; }
        public string SiteUrl { get; internal set; }
        public string UserDescription { get; internal set; }
        public string ProjectName { get; internal set; }
    }
}
