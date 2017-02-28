using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CollAction.ValidationAttributes
{
    public class MaxImageDimensionsAttribute : ValidationAttribute
    {
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public MaxImageDimensionsAttribute(int width, int height)
        {
            _maxWidth = width;
            _maxHeight = height;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            var image = Image.FromStream((value as IFormFile).OpenReadStream());
            return image.Width <= _maxWidth && image.Height <= _maxHeight;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Only images with dimensions of or smaller than {0}x{1}px are accepted.", _maxWidth, _maxHeight);
        }
    }
}