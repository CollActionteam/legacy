using CollAction.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.ValidationAttributes
{
    public sealed class CategoriesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var categories = (ICollection<Category>)value;

            if (!categories.Any())
            {
                return new ValidationResult("You need to specify up to two categories for the project");
            }

            if (categories.Count > 2)
            {
                return new ValidationResult("You can only specify up to two categories");
            }

            if (categories.Distinct().Count() != categories.Count)
            {
                return new ValidationResult("The project has duplicate categories");
            }

            return ValidationResult.Success;
        }
    }
}
