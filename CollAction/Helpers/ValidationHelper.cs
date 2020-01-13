using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.Helpers
{
    public static class ValidationHelper
    {
        public static IEnumerable<ValidationResult> Validate<TItem>(TItem item, IServiceProvider serviceProvider)
        {
            var validationContext = new ValidationContext(item, serviceProvider: serviceProvider, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(item, validationContext, validationResults);
            return validationResults;
        }

        public static IEnumerable<IdentityError> ValidateAsIdentity<TItem>(TItem item, IServiceProvider serviceProvider)
            => Validate(item, serviceProvider).Select(ValidationResultToIdentityError);

        private static IdentityError ValidationResultToIdentityError(ValidationResult result)
            => new IdentityError() 
               {
                   Code = string.Join("_", result.MemberNames), 
                   Description = result.ErrorMessage 
               };
    }
}
