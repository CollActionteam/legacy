using CollAction.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ValidationAttributes
{
    public sealed class FrontendUrlAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Uri canonicalAddress = new Uri(validationContext.GetRequiredService<IOptions<SiteOptions>>().Value.CanonicalAddress);
            if (!(value is string givenAddress))
            {
                return new ValidationResult("Given URL is not a string");
            }
            return canonicalAddress.Host == new Uri(givenAddress).Host ? ValidationResult.Success : new ValidationResult("URL doesn't have a valid host");
        }
    }
}
