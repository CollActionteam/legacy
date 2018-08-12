using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CollAction.ValidationAttributes
{
    public class RichTextRequiredAttribute : RequiredAttribute, IClientModelValidator
    {        
        public void AddValidation(ClientModelValidationContext context)
        {
            var requiredMessage = ErrorMessage ?? $"{context.ModelMetadata.DisplayName} is required";
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-required"] = requiredMessage;
            context.Attributes["data-val-richtextrequired"] = requiredMessage;
        }
    }
}