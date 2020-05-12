using GraphQL.EntityFramework;
using GraphQL.Types;
using CollAction.Data;
using CollAction.Models;

namespace CollAction.GraphQl.Queries
{
    public class CrowdactionCommentGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionComment>
    {
        public CrowdactionCommentGraph(IEfGraphQLService<ApplicationDbContext> graphService) : base(graphService)
        {
            Field(x => x.Id);
            Field<NonNullGraphType<DateTimeOffsetGraphType>>(nameof(CrowdactionComment.CommentedAt), resolve: x => x.Source.CommentedAt);
            Field(x => x.Comment);
            Field<IdGraphType>(nameof(CrowdactionComment.UserId), resolve: x => x.Source.UserId);
            Field<NonNullGraphType<IdGraphType>>(nameof(CrowdactionComment.CrowdactionId), resolve: x => x.Source.CrowdactionId);
            AddNavigationField(nameof(CrowdactionComment.Crowdaction), x => x.Source.Crowdaction);
            AddNavigationField(nameof(CrowdactionComment.User), x => x.Source.User);
        }
    }
}
