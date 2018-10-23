using System.Collections.Generic;

namespace CollAction.Models
{
    public class BrowseProjectsViewModel
    {
        public string OwnerId { get; set; }

        public List<DisplayProjectViewModel> Projects { get; set; }

        public static DisplayProjectViewModel DefaultDisplayProjectViewModel { get { return new DisplayProjectViewModel(); } }
    }
}