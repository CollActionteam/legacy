using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using CollAction.Helpers;


namespace CollAction.ValidationAttributes
{
    public class SecureRichTextAttribute : ValidationAttribute
    {
        public SecureRichTextAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            var html = value as string;

            if (html == null) return true;

            var sanitizer = new HtmlInputValidator(html);

            return !sanitizer.IsSanitized;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format($"The {name} contains invalid HTML tags.");
        }
    }
}