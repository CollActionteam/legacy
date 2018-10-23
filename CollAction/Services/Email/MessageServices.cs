using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using CollAction.Helpers;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Services.Email
{
    public class AuthMessageSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions _authOptions;
        private readonly ILogger<AuthMessageSender> _logger;
        private readonly IBackgroundJobClient _jobClient;

        public AuthMessageSender(IOptions<AuthMessageSenderOptions> authOptions, IBackgroundJobClient jobClient, ILoggerFactory loggerFactory)
        {
            _authOptions = authOptions.Value;
            _logger = loggerFactory.CreateLogger<AuthMessageSender>();
            _jobClient = jobClient;
        }

        public void SendEmails(IEnumerable<string> emails, string subject, string message)
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

            string job = _jobClient.Enqueue(() => SendEmail(emailRequest, emails));

            _logger.LogInformation("sending email to {0} with subject {1} with hangfire job {2}", string.Join(", ", emails), subject, job);
        }

        public void SendEmail(string email, string subject, string message)
            => SendEmails(new[] { email }, subject, message);

        public async Task SendEmail(SendEmailRequest request, IEnumerable<string> emails)
        {
            using (AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(_authOptions.SesAwsAccessKeyID, _authOptions.SesAwsAccessKey, RegionEndpoint.GetBySystemName(_authOptions.SesRegion)))
            {
                SendEmailResponse response = await client.SendEmailAsync(request);
                if (!response.HttpStatusCode.IsSuccess())
                {
                    _logger.LogError("failed to send email to {0}", string.Join(", ", emails));
                    throw new InvalidOperationException($"failed to send email to {string.Join(", ", emails)}, {response.HttpStatusCode}");
                }

                _logger.LogInformation("successfully send email to {0}", string.Join(", ", emails));
            }
        }
    }
}
