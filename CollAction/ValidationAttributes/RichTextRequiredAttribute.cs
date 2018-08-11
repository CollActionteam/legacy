using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CollAction.ValidationAttributes
{
    public class RichTextRequiredAttribute : ValidationAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-richtextrequired"] = ErrorMessage ?? $"{context.ModelMetadata.DisplayName} is required";
        }
    }
}