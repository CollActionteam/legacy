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
        public string DateProperty { get; }
        public int Months { get; }

        public WithinMonthsAfterDatePropertyAttribute(int months, string dateProperty)
        {
            Months = months;
            DateProperty = dateProperty;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            DateTime startDate = GetReferencedDate(validationContext.ObjectInstance);
            DateTime maxDate = startDate.AddMonths(Months);
            DateTime dateToCheck = (DateTime)value;

            if (dateToCheck < startDate || dateToCheck > maxDate)
            {
                return new ValidationResult(ErrorMessage);
            }
            else
            {
                return ValidationResult.Success!;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "dataProperty is an argument of the attribute")]
        private DateTime GetReferencedDate(object instance)
        {
            PropertyInfo? propertyInfo = instance.GetType().GetProperty(DateProperty);
            if (propertyInfo == null || propertyInfo.PropertyType != typeof(DateTime))
            {
                throw new ArgumentException($"The specified property '{DateProperty}' does not refer to a valid DateTime property", nameof(DateProperty));
            }

            object? propValue = propertyInfo.GetValue(instance, null);
            if (propValue == null)
            {
                throw new ArgumentException($"The specified property '{DateProperty}' contained a null value", nameof(DateProperty));
            }

            return (DateTime)propValue;
        }
    }
}