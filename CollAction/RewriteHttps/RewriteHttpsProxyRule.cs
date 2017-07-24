using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace CollAction.RewriteHttps
{
    public class RewriteHttpsProxyRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            HttpRequest request = context.HttpContext.Request;
            if (!IsHttps(request))
            {
                string newUrl =
                    new StringBuilder()
                        .Append("https://")
                        .Append(request.Host)
                        .Append(request.PathBase)
                        .Append(request.Path)
                        .Append(request.QueryString)
                        .ToString();
                context.HttpContext.Response.Redirect(newUrl, true);
            }
        }

        private bool IsHttps(HttpRequest request)
        {
            if (request.IsHttps)
                return true;
            else if (request.Headers.TryGetValue("X-Forwarded-Proto", out StringValues scheme))
                return scheme[0] == "https";
            else
                return false;
        }
    }
}
