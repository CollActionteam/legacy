using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Models.AdminViewModels
{
    public class ManageProjectViewModel
    {
        public Project Project { get; set; }
        public string ProjectTags { get; set; }
        public SelectList UserList { get; set; }
        public SelectList LocationList { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList DisplayPriorityList { get; set; }
    }
}
