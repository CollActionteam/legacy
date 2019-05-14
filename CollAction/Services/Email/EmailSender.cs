using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using CollAction.Helpers;
using CollAction.Services.ViewRender;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions _authOptions;
        private readonly IViewRenderService _viewRenderService;
        private readonly ILogger<EmailSender> _logger;
        private readonly IBackgroundJobClient _jobClient;

        public EmailSender(IOptions<AuthMessageSenderOptions> authOptions, IBackgroundJobClient jobClient, ILoggerFactory loggerFactory, IViewRenderService viewRenderService)
        {
            _authOptions = authOptions.Value;
            _viewRenderService = viewRenderService;
            _logger = loggerFactory.CreateLogger<EmailSender>();
            _jobClient = jobClient;
        }

        public void SendEmails(IEnumerable<string> emails, string subject, string message)
        {
            string job = _jobClient.Enqueue(() => SendEmailQueued(emails, subject, message));
            _logger.LogInformation("sending email to {0} with subject {1} with hangfire job {2}", string.Join(", ", emails), subject, job);
        }

        public void SendEmail(string email, string subject, string message)
            => SendEmails(new[] { email }, subject, message);

        public Task SendEmailsTemplated(IEnumerable<string> emails, string subject, string emailTemplate)
            => SendEmailsTemplated<object>(emails, subject, emailTemplate, new object());

        public Task SendEmailTemplated(string email, string subject, string emailTemplate)
            => SendEmailTemplated<object>(email, subject, emailTemplate, new object());

        public async Task SendEmailsTemplated<TModel>(IEnumerable<string> emails, string subject, string emailTemplate, TModel model)
        {
            string message = await _viewRenderService.Render($"Views/Emails/{emailTemplate}.cshtml", model);
            SendEmails(emails, subject, message);
        }

        public Task SendEmailTemplated<TModel>(string email, string subject, string emailTemplate, TModel model)
            => SendEmailsTemplated(new[] { email }, subject, emailTemplate, model);

        public async Task SendEmailQueued(IEnumerable<string> emails, string subject, string message) // Not part of the interface, but needs to be public so that hangfire can queue it
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

            using (AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(_authOptions.SesAwsAccessKeyID, _authOptions.SesAwsAccessKey, RegionEndpoint.GetBySystemName(_authOptions.SesRegion)))
            {
                SendEmailResponse response = await client.SendEmailAsync(emailRequest);
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
