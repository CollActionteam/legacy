using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace CollAction.ValidationAttributes
{
    public class FileTypeAttribute : ValidationAttribute
    {
        private readonly string[] types;

        public FileTypeAttribute(params string[] types)
        {
            this.types = types;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var extension = Path.GetExtension((value as IFormFile).FileName).ToLower().Substring(1); // Strip off the preceeding dot.
            return types.Contains(extension);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Invalid file type. Only files of type {0} are accepted.", string.Join(", ", types));
        }
    }
}