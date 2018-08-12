using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CollAction.ValidationAttributes
{
    public class WithinMonthsAfterTodayAttribute : ValidationAttribute, IClientModelValidator
    {
        protected readonly int _months;

        public WithinMonthsAfterTodayAttribute(int months)
        {
            _months = months;
        }

        /// <summary>
        ///     returns True if the input date falls within the specified number of months after today's date.
        /// </summary>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null) return ValidationResult.Success;

            var today = GetNow();
            var maxDate = today.AddMonths(_months);
            var checkDate = (DateTime)value;
            if (checkDate < today || checkDate > maxDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var now = GetNow();
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-withinmonthsaftertoday"] = ErrorMessage;
            context.Attributes["data-val-withinmonthsaftertoday-today"] = now.ToString("o"); // Produce ISO 8601 date/times
            context.Attributes["data-val-withinmonthsaftertoday-months"] = _months.ToString();
        }

        private DateTime GetNow()
        {
            return DateTime.UtcNow.Date;
        }
    }
}