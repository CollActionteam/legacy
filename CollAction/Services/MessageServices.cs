using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Services
{
    public class AuthMessageSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions _authOptions;
        private readonly ILogger<AuthMessageSender> _logger;

        public AuthMessageSender(IOptions<AuthMessageSenderOptions> authOptions, ILoggerFactory loggerFactory)
        {
            _authOptions = authOptions.Value;
            _logger = loggerFactory.CreateLogger<AuthMessageSender>();
        }

        public Task SendEmailsAsync(IEnumerable<string> emails, string subject, string message)
        {
            SendEmailRequest emailRequest = new SendEmailRequest()
            {
                Source = _authOptions.FromAddress,
                Destination = new Destination(emails.ToList()),
                Message = new Message()
                {
                    Body = new Body() { Html = new Content(message) },
                    Subject = new Content(subject)
                }
            };

            string job = BackgroundJob.Enqueue(() => SendEmail(emailRequest, emails));

            _logger.LogInformation("sending email to {0} with subject {1} with hangfire job {2}", string.Join(", ", emails), subject, job);

            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string email, string subject, string message)
            => SendEmailsAsync(new[] { email }, subject, message);

        public async Task SendEmail(SendEmailRequest request, IEnumerable<string> emails)
        {
            using (AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(_authOptions.SesAwsAccessKeyID, _authOptions.SesAwsAccessKey, _authOptions.Region))
            {
                SendEmailResponse response = await client.SendEmailAsync(request);
                _logger.LogInformation("successfully send email to {0}", string.Join(", ", emails));
            }
        }
    }
}
