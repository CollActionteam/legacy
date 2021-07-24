using System;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ValidationAttributes
{
    /// <summary>
    /// Validates if the input date falls within the specified number of months after today's date.
    /// </summary>
    public sealed class WithinMonthsAfterTodayAttribute : ValidationAttribute
    {
        public int Months { get; }

        public WithinMonthsAfterTodayAttribute(int months)
        {
            Months = months;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            DateTime today = DateTime.UtcNow.Date;
            var maxDate = today.AddMonths(Months);
            var checkDate = (DateTime)value;
            if (checkDate < today || checkDate > maxDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}