using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CollAction.ValidationAttributes
{
    public class WithinMonthsAfterDatePropertyAttribute : ValidationAttribute
    {
        private readonly int _months;
        private readonly string _dateProperty;

        public WithinMonthsAfterDatePropertyAttribute(int months, string dateProperty)
        {
            _months = months; 
            _dateProperty = dateProperty;
        }

        /// <summary>
        ///     returns True if the input date falls within the specified number of months after the date property being compared to.
        /// </summary>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null) return ValidationResult.Success;

            Object instance = context.ObjectInstance;
            PropertyInfo propertyInfo = instance.GetType().GetProperty(_dateProperty);
            if (propertyInfo == null || propertyInfo.PropertyType != typeof(DateTime))
            {
                throw new ArgumentException(String.Format("The specified property '{0}' does not refer to a valid DateTime property.", _dateProperty));
            }

            DateTime startDate = (DateTime)propertyInfo.GetValue(instance, null);
            DateTime endDate = startDate.AddMonths(_months);
            DateTime checkDate = (DateTime)value;
            if (checkDate <= startDate || checkDate > endDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}