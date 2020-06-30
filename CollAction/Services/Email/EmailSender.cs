using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using CollAction.Helpers;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Services.Email
{
    public sealed class EmailSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions authOptions;
        private readonly ILogger<EmailSender> logger;
        private readonly IBackgroundJobClient jobClient;
        private readonly IRazorLightEngine engine;

        public EmailSender(IOptions<AuthMessageSenderOptions> authOptions, IBackgroundJobClient jobClient, ILogger<EmailSender> logger, IRazorLightEngine engine)
        {
            this.authOptions = authOptions.Value;
            this.logger = logger;
            this.jobClient = jobClient;
            this.engine = engine;
        }

        public void SendEmails(IEnumerable<string> emails, string subject, string message)
        {
            string job = jobClient.Enqueue(() => SendEmailQueued(emails, subject, message));
            logger.LogInformation("sending email to {0} with subject {1} with hangfire job {2}", string.Join(", ", emails), subject, job);
        }

        public void SendEmail(string email, string subject, string message)
        {
            SendEmails(new[] { email }, subject, message);
        }

        public Task SendEmailsTemplated(IEnumerable<string> emails, string subject, string emailTemplate)
            => SendEmailsTemplated(emails, subject, emailTemplate, new object());

        public Task SendEmailTemplated(string email, string subject, string emailTemplate)
            => SendEmailTemplated(email, subject, emailTemplate, new object());

        public async Task SendEmailsTemplated<TModel>(IEnumerable<string> emails, string subject, string emailTemplate, TModel model)
        {
            string message = await engine.CompileRenderAsync($"{emailTemplate}.cshtml", model).ConfigureAwait(false);
            SendEmails(emails, subject, message);
        }

        public Task SendEmailTemplated<TModel>(string email, string subject, string emailTemplate, TModel model)
        {
            return SendEmailsTemplated(new[] { email }, subject, emailTemplate, model);
        }

        public async Task SendEmailQueued(IEnumerable<string> emails, string subject, string message) // Not part of the interface, but needs to be public so that hangfire can queue it
        {
            SendEmailRequest emailRequest = new SendEmailRequest()
            {
                Source = authOptions.FromAddress,
                Destination = new Destination(emails.ToList()),
                Message = new Message()
                {
                    Body = new Body() { Html = new Content(message) },
                    Subject = new Content(subject)
                }
            };

            using AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(authOptions.SesAwsAccessKeyID, authOptions.SesAwsAccessKey, RegionEndpoint.GetBySystemName(authOptions.SesRegion));
            SendEmailResponse response = await client.SendEmailAsync(emailRequest).ConfigureAwait(false);
            if (!response.HttpStatusCode.IsSuccess())
            {
                logger.LogError("failed to send email to {0}", string.Join(", ", emails));
                throw new InvalidOperationException($"failed to send email to {string.Join(", ", emails)}, {response.HttpStatusCode}");
            }

            logger.LogInformation("successfully send email to {0}", string.Join(", ", emails));
        }
    }
}
