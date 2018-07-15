using System.ComponentModel.DataAnnotations;

namespace CollAction.ValidationAttributes
{
    public sealed class MustBeTrueAttribute: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }
    }
}
