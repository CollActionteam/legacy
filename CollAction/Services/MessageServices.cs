using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
        private readonly AuthMessageSenderOptions _authOptions;
        private readonly ILogger<AuthMessageSender> _logger;

        public AuthMessageSender(IOptions<AuthMessageSenderOptions> authOptions, ILoggerFactory loggerFactory)
        {
            _authOptions = authOptions.Value;
            _logger = loggerFactory.CreateLogger<AuthMessageSender>();
        }

        public async Task SendEmailsAsync(IEnumerable<string> emails, string subject, string message)
        {
            _logger.LogInformation("sending email to {0} with subject {1}", string.Join(", ", emails), subject);

            SendEmailRequest emailRequest = new SendEmailRequest()
            {
                Source = _authOptions.FromAddress,
                Destination = new Destination(emails.ToList()),
                Message = new Message()
                {
                    Body = new Body(new Content(message)),
                    Subject = new Content(subject)
                }
            };

            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(_authOptions.SesAwsAccessKeyID, _authOptions.SesAwsAccessKey, _authOptions.Region);
            SendEmailResponse response = await client.SendEmailAsync(emailRequest);

            if (!(new[] { HttpStatusCode.OK, HttpStatusCode.Accepted }.Contains(response.HttpStatusCode)))
                _logger.LogError("failed to send email to {0} with response {1}, MessageId: {2}, RequestId: {3}, MetaData: {4}", string.Join(", ", emails), response.HttpStatusCode, response.MessageId, response.ResponseMetadata.RequestId, response.ResponseMetadata.Metadata.Select(p => $"{{{p.Key}-{p.Value}}}"));
            else
                _logger.LogInformation("successfully send email to {0}", string.Join(", ", emails));
        }

        public Task SendEmailAsync(string email, string subject, string message)
            => SendEmailsAsync(new[] { email }, subject, message);

        public Task SendSmsAsync(string number, string message)
        {
            throw new NotImplementedException();
            // Plug in your SMS service here to send a text message.
        }
    }
}
