using CollAction.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace CollAction.ValidationAttributes
{
    public class MinImageDimensionsAttribute : ValidationAttribute
    {
        private readonly int _minWidth;
        private readonly int _minHeight;

        public MinImageDimensionsAttribute(string widthByHeight)
        {
            var dimensions = widthByHeight.Split('x').Select(t => Convert.ToInt32(t)).ToList();
            _minWidth = dimensions[0];
            _minHeight = dimensions[1];
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            var metaDataReader = new SDImageMetaDataReader();
            metaDataReader.ReadFromStream((value as IFormFile).OpenReadStream());
            return metaDataReader.Width >= _minWidth && metaDataReader.Height >= _minHeight;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Only images with dimensions of or larger than {0}x{1}px are accepted.", _minWidth, _minHeight);
        }
    }
}