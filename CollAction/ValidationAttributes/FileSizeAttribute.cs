using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace CollAction.ValidationAttributes
{
    public sealed class FileSizeAttribute : ValidationAttribute
    {
        public int MaxSize { get; }

        public FileSizeAttribute(int maxSize)
        {
            MaxSize = maxSize;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (!(value is IFormFile))
            {
                throw new ArgumentException("Value being validated is not a IFormFile", nameof(value));
            }

            return ((IFormFile)value).Length <= MaxSize;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The file size should not exceed {MaxSize} bytes.";
        }
    }
}