using Microsoft.Extensions.Options;
using SendGrid;
using Serilog;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CollAction.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IOptions<AuthMessageSenderOptions> _authOptions;

        public AuthMessageSender(IOptions<AuthMessageSenderOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            Log.Information("sending email to {0} with subject {1}", email, subject);
            SendGridMessage gridMessage = new SendGridMessage()
            {
                From = new MailAddress(_authOptions.Value.FromAddress, _authOptions.Value.FromName),
                Subject = subject,
                Text = message,
                Html = message
            };
            gridMessage.AddTo(email);

            return new Web(_authOptions.Value.SendGridKey).DeliverAsync(gridMessage);
        }

        public Task SendSmsAsync(string number, string message)
        {
            throw new NotImplementedException();
            // Plug in your SMS service here to send a text message.
        }
    }
}
