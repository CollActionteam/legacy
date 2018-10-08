using System.Collections.Generic;

namespace CollAction.Services.Email
{
    public interface IEmailSender
    {
        void SendEmails(IEnumerable<string> emails, string subject, string message);
        void SendEmail(string email, string subject, string message);
    }
}
