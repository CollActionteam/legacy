using System;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Models.ProjectViewModels
{
    public sealed class SendProjectEmail
    {
        public int ProjectId { get; set; }
        [Display(Name = "Onderwerp")]
        public string Subject { get; set; }
        [Display(Name = "Bericht")]
        public string Message { get; set; }
        public Project Project { get; set; }
        public int EmailsAllowedToSend { get; set; }
        public DateTime SendEmailsUntil { get; internal set; }
    }
}
