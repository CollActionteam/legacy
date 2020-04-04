﻿using CollAction.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
            else if (!validationContext.GetRequiredService<IWebHostEnvironment>().IsProduction())
            {
                return ValidationResult.Success; // We only check this stuff is production environments
            }

            string publicAddress = validationContext.GetRequiredService<IOptions<SiteOptions>>().Value.PublicAddress;
            try
            {
                if (!(value is string givenAddress))
                {
                    return new ValidationResult("Given URL is not a string");
                }
                Uri givenAddressUri = new Uri(givenAddress);
                Uri publicAddressUri = new Uri(publicAddress);
                return givenAddressUri.Host == publicAddressUri.Host && givenAddressUri.Scheme == publicAddressUri.Scheme && givenAddressUri.Port == publicAddressUri.Port ?
                           ValidationResult.Success : 
                           new ValidationResult($"This URL isn't allowed: '{givenAddressUri}', '{publicAddressUri}");
            }
            catch (UriFormatException e)
            {
                return new ValidationResult($"{e.Message}: '{value}', '{publicAddress}'");
            }
        }
    }
}