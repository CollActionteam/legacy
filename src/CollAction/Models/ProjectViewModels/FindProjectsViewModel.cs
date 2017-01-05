using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class FindProjectsViewModel
    {
        public List<DisplayProjectViewModel> Projects { get; set; }

        [Display(Name = "Search Text")]
        [Required(ErrorMessage = "Please enter a search term")]
        public string SearchText { get; set; }
    }
}