using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace CollAction.Helpers
{
    public static class ProjectNormalizeStringExtensions
    {
        private static Regex _spaceRemoveRegex = new Regex(@"\s", RegexOptions.Compiled);
        private static Regex _invalidCharRemoveRegex = new Regex(@"[^a-z0-9\s-_]",RegexOptions.Compiled);
        private static Regex _doubleDashRemoveRegex = new Regex(@"([-_]){2,}", RegexOptions.Compiled);

        public static string ToUrlSlug(this string value)
        {                      
            value = value.ToLowerInvariant();
            value = _spaceRemoveRegex.Replace(value, "-");
            value = _invalidCharRemoveRegex.Replace(value, "");
            value = value.Trim('-', '_');
            value = _doubleDashRemoveRegex.Replace(value, "$1");
            if(value.Length == 0)
            {
                value = "-";
            }

            return value;
        }

        public static string RemoveDiacriticsFromString(this string text) 
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                        stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string NormalizeUriPart(this string project)
        {
            return project
                .RemoveDiacriticsFromString()
                .ToUrlSlug();
        }
    }
}