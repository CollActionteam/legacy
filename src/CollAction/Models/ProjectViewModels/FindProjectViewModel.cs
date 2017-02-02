using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class FindProjectsViewModel
    {
        public string OwnerId { get; set; }

        public List<DisplayProjectViewModel> Projects { get; set; }

        [Display(Name = "Search Text")]
        [Required(ErrorMessage = "Please enter a search term")]
        public string SearchText { get; set; }

        public static DisplayProjectViewModel DefaultDisplayProjectViewModel { get { return new DisplayProjectViewModel(); } }
    }
}