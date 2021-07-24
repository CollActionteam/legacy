using CollAction.Services.HtmlValidator;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ValidationAttributes
{
    /// <summary>
    /// Validates that the text is "secure" html (from a rich text editor component most likely)
    /// </summary>
    public sealed class SecureHtmlAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
            => $"The {name} contains HTML tags that are not allowed";

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            bool isValid = value == null || validationContext.GetRequiredService<IHtmlInputValidator>().IsSafe((string)value);
            if (isValid)
            {
                return ValidationResult.Success!;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }
    }
}