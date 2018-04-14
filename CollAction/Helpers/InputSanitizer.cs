using Ganss.XSS;

namespace CollAction.Helpers
{
    public static class InputSanitizer
    {
        public static string Sanitize(string input)
        {
            var saniziter = new HtmlSanitizer(
                allowedTags: new[] { "p", "br", "strong", "em", "i", "u", "a", "ol", "ul", "li" },
                allowedSchemes: new string[] { "http", "https"},
                allowedAttributes: new[] { "target" },
                uriAttributes: new[] { "href" },
                allowedCssProperties: new string[] {},
                allowedCssClasses: new string[] {}
            );

            return saniziter.Sanitize(input);
        }
    }
}