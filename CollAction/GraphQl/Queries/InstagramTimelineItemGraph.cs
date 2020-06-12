using CollAction.Services.Instagram.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class InstagramTimelineItemGraph : ObjectGraphType<InstagramTimelineItem>
    {
        public InstagramTimelineItemGraph()
        {
            Field(x => x.ShortCode);
            Field(x => x.Link);
            Field(x => x.Date);
            Field(x => x.AccessibilityCaption, true);
            Field(x => x.Caption, true);
            Field(x => x.ThumbnailSrc);
        }
    }
}
