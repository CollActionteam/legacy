using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace CollAction.ValidationAttributes
{
    public sealed class TagsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var tags = (ICollection<string>)value;

            bool isValid = tags.All(t => Regex.IsMatch(t, @"^[A-Za-z][A-Za-z0-9_-]{0,29}$"));

            if (!isValid)
            {
                return new ValidationResult("Tags must be between one and thirty characters, start with a letter, and only contain letters, numbers, underscores or dashes");
            }

            return ValidationResult.Success;
        }
    }
}
