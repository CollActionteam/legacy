using Ganss.XSS;

namespace CollAction.Helpers
{
    public static class InputSanitizer
    {
        public static string Sanitize(string input)
        {
            var saniziter = new HtmlSanitizer(
                allowedTags: new[] { "p", "br", "strong", "em", "i", "u", "a", "ol", "ul", "li" },
                allowedAttributes: new[] { "href", "target" },
                allowedCssClasses: new string[] {},
                allowedCssProperties: new string[] {}                    
            );

            return saniziter.Sanitize(input);

        }
    }
}