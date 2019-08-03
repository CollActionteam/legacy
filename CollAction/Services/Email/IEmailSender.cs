using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollAction.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailsTemplated<TModel>(IEnumerable<string> emails, string subject, string emailTemplate, TModel model);

        Task SendEmailTemplated<TModel>(string email, string subject, string emailTemplate, TModel model);

        Task SendEmailsTemplated(IEnumerable<string> emails, string subject, string emailTemplate);

        Task SendEmailTemplated(string email, string subject, string emailTemplate);

        void SendEmails(IEnumerable<string> emails, string subject, string message);

        void SendEmail(string email, string subject, string message);
    }
}
