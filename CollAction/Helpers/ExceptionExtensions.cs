using System;
using System.Linq;

namespace CollAction.Helpers
{
    public static class ExceptionExtensions
    {
        public static string GetExceptionDetails(this Exception exception)
        {
            var fields =
                exception.GetType()
                         .GetProperties()
                         .Select(
                             property =>
                             {
                                 object propertyValue = property.GetValue(exception);
                                 string value;
                                 if (property.Name == nameof(Exception.InnerException))
                                 {
                                     value = GetExceptionDetails((Exception)propertyValue);
                                 }
                                 else
                                 {
                                     value = propertyValue.ToString();
                                 }
                                 return $"{property.Name} = {value}";
                             });
            return string.Join(Environment.NewLine, fields);
        }
    }
}
