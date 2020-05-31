using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CollAction.ValidationAttributes
{
    /// <summary>
    /// Validates if the input date falls within the specified number of months after the date property being compared to.
    /// </summary>
    public sealed class WithinMonthsAfterDatePropertyAttribute : ValidationAttribute
    {
        private readonly string dateProperty;
        private readonly int months;

        public WithinMonthsAfterDatePropertyAttribute(int months, string dateProperty)
        {
            this.months = months;
            this.dateProperty = dateProperty;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext context)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            DateTime startDate = GetReferencedDate(context.ObjectInstance);
            DateTime maxDate = startDate.AddMonths(months);
            DateTime dateToCheck = (DateTime)value;

            if (dateToCheck < startDate || dateToCheck > maxDate)
            {
                return new ValidationResult(ErrorMessage);
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "dataProperty is an argument of the attribute")]
        private DateTime GetReferencedDate(object instance)
        {
            PropertyInfo? propertyInfo = instance.GetType().GetProperty(dateProperty);
            if (propertyInfo == null || propertyInfo.PropertyType != typeof(DateTime))
            {
                throw new ArgumentException($"The specified property '{dateProperty}' does not refer to a valid DateTime property", nameof(dateProperty));
            }

            object? propValue = propertyInfo.GetValue(instance, null);
            if (propValue == null)
            {
                throw new ArgumentException($"The specified property '{dateProperty}' contained a null value", nameof(dateProperty));
            }

            return (DateTime)propValue;
        }
    }
}