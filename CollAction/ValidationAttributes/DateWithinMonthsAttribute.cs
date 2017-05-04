using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CollAction.ValidationAttributes
{
    public class DateWithinMonthsAttribute : ValidationAttribute
    {
        private readonly int _months;

        public String OfDateProperty { get; set; }

        public DateWithinMonthsAttribute(int months)
        {
            _months = months;
        }

        /// <summary>
        ///     If no {OfDateProperty} is specified, then this date is valid if it falls withing {months} of todays date (not including today).
        /// </summary>
        /// <param months="number of months within which this date must lie"></param>
        /// <param OfDateProperty="a DateTime property in this model"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null) return ValidationResult.Success;

            DateTime startDate = DateTime.UtcNow;
            if (OfDateProperty != null)
            {
                Object instance = context.ObjectInstance;
                PropertyInfo propertyInfo = instance.GetType().GetProperty(OfDateProperty);
                startDate = (DateTime)propertyInfo.GetValue(instance, null);
            }

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