using System;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ValidationAttributes
{
    /// <summary>
    /// Validates if the input date falls within the specified number of months after today's date.
    /// </summary>
    public sealed class WithinMonthsAfterTodayAttribute : ValidationAttribute
    {
        private readonly int months;

        public WithinMonthsAfterTodayAttribute(int months)
        {
            this.months = months;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            DateTime today = DateTime.UtcNow.Date;
            var maxDate = today.AddMonths(months);
            var checkDate = (DateTime)value;
            if (checkDate < today || checkDate > maxDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}