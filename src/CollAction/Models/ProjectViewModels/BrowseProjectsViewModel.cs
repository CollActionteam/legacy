using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class BrowseProjectsViewModel
    {
        public string OwnerId { get; set; }

        public List<DisplayProjectViewModel> Projects { get; set; }

        public static DisplayProjectViewModel DefaultDisplayProjectViewModel { get { return new DisplayProjectViewModel(); } }
    }
}