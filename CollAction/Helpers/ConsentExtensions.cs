using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Linq;

namespace CollAction.Helpers
{
    /*
     * These are extensions on the asp consent features. The standard asp consent is very.. limited (no ability to deny, no ability to deny parts of the cookies), so these extensions add the ability to deny the cookie dialog, and use internal knowledge of the feature (bit of a hack, but better than writing new cookie middleware).
     */
    public static class ConsentExtensions
    {
        private const string AcceptValue = "yes";
        private const string DenyValue = "no";

        public static string CreateDenyCookie(this ITrackingConsentFeature trackingConsentFeature)
            => trackingConsentFeature.CreateConsentCookie().Replace(AcceptValue, DenyValue, StringComparison.Ordinal);

        public static bool ShowCookieDialog(this ITrackingConsentFeature trackingConsentFeature, HttpRequest request)
            => !trackingConsentFeature.IsTrackingDenied(request) && !trackingConsentFeature.CanTrack;

        public static bool IsTrackingDenied(this ITrackingConsentFeature trackingConsentFeature, HttpRequest request)
        {
            string consentCookieKey = trackingConsentFeature.CreateConsentCookie().Split("=").First();
            return request.Cookies[consentCookieKey] == DenyValue;
        }
    }
}
