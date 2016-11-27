using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class UserProfileViewModel
    {
        public List<Project> SubscribedProjects { get; set; }
        public List<Project> CreatedProjects { get; set; }
    }
}