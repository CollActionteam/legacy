using ImageSharp;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            using (Stream imageStream = (value as IFormFile).OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imageStream.CopyTo(ms);
                    try
                    {
                        using (Image image = Image.Load(ms.ToArray()))
                        {
                            return image.Width <= _maxWidth && image.Height <= _maxHeight;
                        }
                    }
                    catch (NotSupportedException)
                    {
                        return false;
                    }
                }
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Only images with dimensions of or smaller than {0}x{1}px are accepted.", _maxWidth, _maxHeight);
        }
    }
}