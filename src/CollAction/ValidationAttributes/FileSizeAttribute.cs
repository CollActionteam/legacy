using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ValidationAttributes
{
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;

        public FileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            return (value as IFormFile).Length <= _maxSize;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("The file size should not exceed {0} bytes.", _maxSize);
        }
    }
}