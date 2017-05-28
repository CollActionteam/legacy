using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public interface IEmailSender
    {
        Task SendEmailsAsync(IEnumerable<string> emails, string subject, string message);
        Task SendEmailAsync(string email, string subject, string message);
    }
}
