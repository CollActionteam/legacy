using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CollAction.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IOptions<AuthMessageSenderOptions> _authOptions;
        private readonly ILogger<AuthMessageSender> _logger;

        public AuthMessageSender(IOptions<AuthMessageSenderOptions> authOptions, ILoggerFactory loggerFactory)
        {
            _authOptions = authOptions;
            _logger = loggerFactory.CreateLogger<AuthMessageSender>();
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            _logger.LogInformation("sending email to {0} with subject {1}", email, subject);
            SendGridMessage gridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_authOptions.Value.FromAddress, _authOptions.Value.FromName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            gridMessage.AddTo(new EmailAddress(email));
            SendGridClient client = new SendGridClient(_authOptions.Value.SendGridKey);
            Response res = await client.SendEmailAsync(gridMessage);

            if (!(new[] { HttpStatusCode.OK, HttpStatusCode.Accepted }.Contains(res.StatusCode)))
            {
                string headers = string.Join(", ", res.Headers.Concat(res.Body.Headers).Select(header => $"[Type: {header.Key}:{string.Join(", ", header.Value)}]"));
                string body = await res.Body.ReadAsStringAsync();
                _logger.LogError("failed to send email to {0} with response {1}, Headers: {2}, Body: {3}", email, res.StatusCode, headers, body);
            }
            else
                _logger.LogInformation("successfully send email to {0}", email);
        }

        public Task SendSmsAsync(string number, string message)
        {
            throw new NotImplementedException();
            // Plug in your SMS service here to send a text message.
        }
    }
}
