using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace CollAction.ValidationAttributes
{
    public sealed class FileTypeAttribute : ValidationAttribute
    {
        public string[] Types { get; }

        public FileTypeAttribute(params string[] types)
        {
            Types = types;
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

            string extension = Path.GetExtension(((IFormFile)value).FileName).ToLowerInvariant()[1..]; // Strip off the preceeding dot.
            return Types.Contains(extension);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Invalid file type. Only files of type {string.Join(", ", Types)} are accepted";
        }
    }
}