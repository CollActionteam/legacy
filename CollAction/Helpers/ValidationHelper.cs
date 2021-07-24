using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.Helpers
{
    public static class ValidationHelper
    {
        public static IEnumerable<ValidationResult> Validate<TItem>(TItem item, IServiceProvider serviceProvider) where TItem: class
        {
            var validationContext = new ValidationContext(item, serviceProvider: serviceProvider, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(item, validationContext, validationResults, true);
            return validationResults;
        }

        public static IEnumerable<IdentityError> ValidateAsIdentity<TItem>(TItem item, IServiceProvider serviceProvider) where TItem: class
            => Validate(item, serviceProvider).Select(ValidationResultToIdentityError);

        public static string GetValidationString(this ModelStateDictionary modelState)
            => string.Join(", ", modelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage));

        private static IdentityError ValidationResultToIdentityError(ValidationResult result)
            => new()
            {
                Code = string.Join("_", result.MemberNames),
                Description = result.ErrorMessage
            };
    }
}
