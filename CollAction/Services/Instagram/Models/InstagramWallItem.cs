using System;

namespace CollAction.Services.Instagram.Models
{
    public sealed class InstagramWallItem
    {
        public InstagramWallItem(string shortCode, string thumbnailSrc, string? accessibilityCaption, string? caption, DateTimeOffset date)
        {
            ShortCode = shortCode;
            ThumbnailSrc = thumbnailSrc;
            AccessibilityCaption = accessibilityCaption;
            Caption = caption;
            Date = date;
        }

        public string ShortCode { get; }

        public string ThumbnailSrc { get; }

        public string? AccessibilityCaption { get; }

        public string? Caption { get; }

        public DateTimeOffset Date { get; }

        public string Link
            => $"https://www.instagram.com/p/{ShortCode}";
    }
}
