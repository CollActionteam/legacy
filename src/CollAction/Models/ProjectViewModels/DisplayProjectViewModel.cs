using System;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models
{
    public class DisplayProjectViewModel
    {
        public Project Project;

        public int Participants;

        [Display(Name = "Progress")]
        public int Progress
        {
            get
            {
                return Participants * 100 / Project.Target;
            }                    
        }
    }
}