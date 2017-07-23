using Microsoft.AspNetCore.Rewrite;

namespace CollAction.RewriteHttps
{
    public static class RewriteExtensions
    {
        public static RewriteOptions AddRewriteHttpsProxyRule(this RewriteOptions options)
        {
            options.Rules.Add(new RewriteHttpsProxyRule());
            return options;
        }
    }
}
