using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CollAction.ValidationAttributes
{
    // Checks for the following form of YouTube link: http://www.youtube.com/watch?v=-wtIMTCHWuI
    public class YouTubeLinkAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            string link = value as string;
            if (!Uri.IsWellFormedUriString(link, UriKind.Absolute)) { return false; }

            Uri uri = new Uri(link);
            if (uri.Scheme != "http" && uri.Scheme != "https") { return false; }
            
            // Check for YouTube link http://www.youtube.com/watch?v=-wtIMTCHWuI
            if (uri.Host == "www.youtube.com" &&
                uri.Segments.Length == 2 &&
                uri.Segments[1] == "watch")
            {
                var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
                if (queryDictionary.Count == 1)
                {
                    Microsoft.Extensions.Primitives.StringValues youTubeId;
                    return queryDictionary.TryGetValue("v", out youTubeId) && IsValidYouTubeId(youTubeId);
                }
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Only YouTube links of the form http://www.youtube.com/watch?v=<your-11-character-video-id> are accepted.");
        }

        private bool IsValidYouTubeId(string youTubeId)
        {
            // Check that the youTubeId query parameter conforms to an 11 alphanumeric character string with underscores '_' and dashes '-' accepted.
            return (new Regex(@"^(?:\w|-){11}?$")).Match(youTubeId).Success;
        }
    }
}
