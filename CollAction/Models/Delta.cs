using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Newtonsoft.Json;

namespace CollAction.Models
{
    public class Delta
    {
        public List<Op> ops { get; set; }

        public static Delta JsonStringToDelta(string jsonString)
        {
            if (String.IsNullOrEmpty(jsonString))
                return null;
            return JsonConvert.DeserializeObject<Delta>(jsonString);
        }

        public static string DeltaToJsonString(Delta delta)
        {
            if (delta == null)
                return String.Empty;
            return JsonConvert.SerializeObject(delta, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static string DeltaToHTML(Delta delta)
        {
            if (delta == null)
                return String.Empty;
            string text;
            DeltaAttributes attributes;
            StringBuilder html = new StringBuilder();
            html.Append("<p>");
            foreach (Op op in delta.ops)
            {
                text = op.insert;
                attributes = op.attributes;
                html.Append(ApplyDeltaAttribute(text, attributes));
            }
            html.Length -= 3;
            return html.ToString();
        }

        public static string ApplyDeltaAttribute(string insert, DeltaAttributes attribute)
        {
            string text = insert;
            if (attribute != null)
            {
                if (attribute.bold == true)
                {
                    text = "<strong>" + text + "</strong>";
                }
                if (attribute.italic == true)
                {
                    text = "<em>" + text + "</em>";
                }
                if (attribute.underline == true)
                {
                    text = "<u>" + text + "</u>";
                }
                if (!String.IsNullOrEmpty(attribute.link))
                {
                    text = "<a href='" + attribute.link + "' target='_blank'>" + text + "</a>";
                }
            }

            return text.Replace("\n", "</p><p>");

        }
    }

    public class Op
    {
        private string _insert;
        public string insert
        {
            get
            {
                return _insert;
            }
            set
            {
                _insert = sanitizeInsert(value);
            }
        }

        public DeltaAttributes attributes { get; set; }

        private string sanitizeInsert(string insert)
        {
            return HtmlEncoder.Default.Encode(insert).Replace("&#xA;", "\n");
        }
    }

    public class DeltaAttributes
    {
        private string _link;
        public string link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = sanitizeLink(value);
            }
        }
        public bool? bold { get; set; }
        public bool? italic { get; set; }
        public bool? underline { get; set; }

        private string sanitizeLink(string insert)
        {
            try {
                Uri uri = new Uri(insert);
                return uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
    }

}
