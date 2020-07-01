using GraphQL.EntityFramework;
using GraphQL.Types;
using CollAction.Data;
using CollAction.Models;
using System;

namespace CollAction.GraphQl.Queries
{
    public class CrowdactionCommentGraph : EfObjectGraphType<ApplicationDbContext, CrowdactionComment>
    {
        public CrowdactionCommentGraph(IEfGraphQLService<ApplicationDbContext> graphService) : base(graphService)
        {
            Field<NonNullGraphType<IdGraphType>, int>(nameof(CrowdactionComment.Id)).Resolve(x => x.Source.Id);
            Field<NonNullGraphType<DateTimeOffsetGraphType>, DateTime>(nameof(CrowdactionComment.CommentedAt)).Resolve(x => x.Source.CommentedAt);
            Field(x => x.Comment);
            Field(x => x.Status);
            Field<IdGraphType, string?>(nameof(CrowdactionComment.UserId)).Resolve(x => x.Source.UserId);
            Field<NonNullGraphType<IdGraphType>, int>(nameof(CrowdactionComment.CrowdactionId)).Resolve(x => x.Source.CrowdactionId);
            AddNavigationField(nameof(CrowdactionComment.Crowdaction), x => x.Source.Crowdaction);
            AddNavigationField(nameof(CrowdactionComment.User), x => x.Source.User, typeof(RestrictedApplicationUserGraph));
        }
    }
}
