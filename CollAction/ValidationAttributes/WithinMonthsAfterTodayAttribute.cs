using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CollAction.ValidationAttributes
{
    public class WithinMonthsAfterTodayAttribute : ValidationAttribute
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

            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddMonths(_months);
            DateTime checkDate = (DateTime)value;
            if (checkDate < startDate || checkDate > endDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}