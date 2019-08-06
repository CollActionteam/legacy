using Ganss.XSS;
using System.ComponentModel.DataAnnotations;

namespace CollAction.Services.HtmlValidator
{
    public class HtmlInputValidator : IHtmlInputValidator
    {
        public bool IsSafe(string inputHtml)
        {
            var sanitizer = new HtmlSanitizer(
                allowedTags: 
                    new[] 
                    {
                        "p",
                        "br",
                        "strong",
                        "em",
                        "i",
                        "u",
                        "a",
                        "ol",
                        "ul",
                        "li"
                    },
                allowedSchemes: 
                    new string[]
                    {
                        "http",
                        "https"
                    },
                allowedAttributes: 
                    new[] 
                    {
                        "target",
                        "href"
                    },
                uriAttributes: 
                    new[] 
                    {
                        "href"
                    },
                allowedCssProperties: 
                    new string[0],
                allowedCssClasses: 
                    new string[0]);

            bool isSafe = true;

            sanitizer.RemovingTag += (a, b) => { isSafe = false; };

            sanitizer.RemovingAttribute += (a, b) => { isSafe = false; };

            sanitizer.RemovingStyle += (a, b) => { isSafe = false; };

            sanitizer.RemovingCssClass += (a, b) => { isSafe = false; };

            sanitizer.RemovingAtRule += (a, b) => { isSafe = false; };

            sanitizer.RemovingComment += (a, b) => { isSafe = false; };

            string output = sanitizer.Sanitize(inputHtml);

            // Check assertion, if sanitized, output must change, and other way around
            if ((output == inputHtml) != isSafe) 
            {
                throw new ValidationException("Html sanitized but no event handler fired");
            }

            return isSafe;
        }
    }
}