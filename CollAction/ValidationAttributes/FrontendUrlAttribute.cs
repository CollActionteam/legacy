using CollAction.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollAction.ValidationAttributes
{
    public sealed class FrontendUrlAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // null should be handled with [Required]
            }

            try
            {
                if (!(value is string givenAddress))
                {
                    return new ValidationResult("Given URL is not a string");
                }
                string givenAddressHost = new Uri(givenAddress).Host;
                var publicAddresses = validationContext.GetRequiredService<IOptions<SiteOptions>>().Value.PublicAddresses;
                return publicAddresses.Any(address => new Uri(address).Host == givenAddressHost) ? 
                           ValidationResult.Success : 
                           new ValidationResult($"URL doesn't have an allowed hostname: {givenAddressHost}");
            }
            catch (UriFormatException e)
            {
                return new ValidationResult($"{e.Message}: '{value}'");
            }
        }
    }
}
