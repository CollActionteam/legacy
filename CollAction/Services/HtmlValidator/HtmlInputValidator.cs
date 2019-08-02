using Ganss.XSS;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Helpers
{
    public class HtmlInputValidator : IHtmlInputValidator
    {
        public bool IsSafe(string inputHtml)
        {
            var sanitizer = new HtmlSanitizer(
                allowedTags: new[] { "p", "br", "strong", "em", "i", "u", "a", "ol", "ul", "li" },
                allowedSchemes: new string[] { "http", "https" },
                allowedAttributes: new[] { "target", "href" },
                uriAttributes: new[] { "href" },
                allowedCssProperties: new string[] { },
                allowedCssClasses: new string[] { }
            );

            bool isSafe = true;
            sanitizer.RemovingTag += (a, b) => { isSafe = false; };
            sanitizer.RemovingAttribute += (a, b) => { isSafe = false; };
            sanitizer.RemovingStyle += (a, b) => { isSafe = false; };
            sanitizer.RemovingCssClass += (a, b) => { isSafe = false; };
            sanitizer.RemovingAtRule += (a, b) => { isSafe = false; };
            sanitizer.RemovingComment += (a, b) => { isSafe = false; };

            string output = sanitizer.Sanitize(inputHtml);
            if ((output == inputHtml) == isSafe) // Check assertion, if sanitized, output must change, and other way around
            {
                throw new ValidationException("Html sanitized but no event handler fired");
            }

            return isSafe;
        }
    }
}