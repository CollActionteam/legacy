// using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Threading.Tasks;

namespace CollAction.Models
{
    public class FindProjectViewModel
    {
        [Display(Name = "Search Text")]
        [Required(ErrorMessage = "Please enter a search term")]
        public string SearchText { get; set; }
        public List<Project> Projects { get; set; }
    }
}