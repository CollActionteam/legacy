using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CollAction.ValidationAttributes
{
    public class WithinMonthsAfterDatePropertyAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _dateProperty;

        private readonly int _months;

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

            var startDate = GetReferencedDate(context.ObjectInstance);
            var maxDate = GetMaxDate(startDate);            
            var checkDate = (DateTime)value;
            if (checkDate <= startDate || checkDate > maxDate)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var now = DateTime.Now;
            var duration = now.AddMonths(_months) - now;

            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-WithinMonthsAfterDate"] = ErrorMessage;
            context.Attributes["data-val-WithinMonthsAfterDate-propertyname"] = _dateProperty;
            context.Attributes["data-val-WithinMonthsAfterDate-months"] = _months.ToString();
        }

        private DateTime GetReferencedDate(Object instance)
        {
            PropertyInfo propertyInfo = instance.GetType().GetProperty(_dateProperty);
            if (propertyInfo == null || propertyInfo.PropertyType != typeof(DateTime))
            {
                throw new ArgumentException(String.Format("The specified property '{0}' does not refer to a valid DateTime property.", _dateProperty));
            }

            return (DateTime)propertyInfo.GetValue(instance, null);
        }

        private DateTime GetMaxDate(DateTime startDate)
        {
            return startDate.AddMonths(_months);
        }
    }
}