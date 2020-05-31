using MailChimp.Net.Core;
using System;

namespace CollAction.Services.Newsletter
{
    public sealed class NeedsToResubscribeException : Exception
    {
        public NeedsToResubscribeException(MailChimpException innerException) : base("User needs to resubscribe", innerException)
        {
        }

        public NeedsToResubscribeException()
        {
        }

        public NeedsToResubscribeException(string message) : base(message)
        {
        }

        public NeedsToResubscribeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
