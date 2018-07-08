using Ganss.XSS;

namespace CollAction.Helpers
{
    public class HtmlInputValidator
    {
        private HtmlSanitizer _sanitizer;

        public bool IsSanitized { get; private set; } = false;

        public string SanitizedOutput { get; private set; }

        public HtmlInputValidator(string input)
        {
            _sanitizer = new HtmlSanitizer(
                allowedTags: new[] { "p", "br", "strong", "em", "i", "u", "a", "ol", "ul", "li" },
                allowedSchemes: new string[] { "http", "https"},
                allowedAttributes: new[] { "target", "href" },
                uriAttributes: new[] { "href" },
                allowedCssProperties: new string[] {},
                allowedCssClasses: new string[] {}
            );

            _sanitizer.RemovingTag += Sanitize;
            _sanitizer.RemovingAttribute += Sanitize;
            _sanitizer.RemovingStyle += Sanitize;
            _sanitizer.RemovingCssClass += Sanitize;
            _sanitizer.RemovingAtRule += Sanitize;
            _sanitizer.RemovingComment += Sanitize;

            SanitizedOutput = _sanitizer.Sanitize(input);
        }

        private void Sanitize(object source, object evt)
        {
            IsSanitized = true;
        }
    }
}