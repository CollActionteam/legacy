using System;

namespace CollAction.Models.ProjectViewModels
{
    public sealed class SendProjectEmail
    {
        public int ProjectId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Project Project { get; set; }
        public int EmailsAllowedToSend { get; set; }
        public DateTime SendEmailsUntil { get; internal set; }
    }
}
