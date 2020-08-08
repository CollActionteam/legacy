using CollAction.Data;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Services.Crowdactions;
using CollAction.Services.Instagram;
using CollAction.Services.Instagram.Models;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.GraphQl.Queries
{
    public sealed class CrowdactionGraph : EfObjectGraphType<ApplicationDbContext, Crowdaction>
    {
        public CrowdactionGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>, int>(nameof(Crowdaction.Id)).Resolve(x => x.Source.Id);
            Field(x => x.AnonymousUserParticipants);
            Field(x => x.BannerImageFileId, true);
            Field(x => x.CreatorComments, true);
            Field(x => x.Description);
            Field(x => x.DescriptionVideoLink, true);
            Field(x => x.DescriptiveImageFileId, true);
            Field(x => x.CardImageFileId, true);
            Field(x => x.DisplayPriority);
            Field<NonNullGraphType<DateTimeOffsetGraphType>, DateTime>(nameof(Crowdaction.End)).Resolve(x => x.Source.End);
            Field(x => x.Goal);
            Field(x => x.IsActive);
            Field(x => x.IsClosed);
            Field(x => x.IsComingSoon);
            Field(x => x.Name);
            Field(x => x.NumberCrowdactionEmailsSent);
            Field(x => x.InstagramUser, true);
            Field<IdGraphType, string?>(nameof(Crowdaction.OwnerId)).Resolve(x => x.Source.OwnerId);
            Field(x => x.Proposal);
            Field(x => x.RemainingTime);
            Field<NonNullGraphType<DateTimeOffsetGraphType>, DateTime>(nameof(Crowdaction.Start)).Resolve(x => x.Source.Start);
            Field(x => x.Status);
            Field(x => x.Target);
            Field(x => x.NameNormalized);
            Field<NonNullGraphType<StringGraphType>, string>(nameof(Crowdaction.Url)).Resolve(p => p.Source.Url.ToString());
            Field<NonNullGraphType<BooleanGraphType>, bool>("canSendCrowdactionEmail")
                .Resolve(c =>
                {
                    return c.GetUserContext().ServiceProvider.GetRequiredService<ICrowdactionService>().CanSendCrowdactionEmail(c.Source);
                });
            FieldAsync<NonNullGraphType<StringGraphType>, string>(
                nameof(Crowdaction.RemainingTimeUserFriendly),
                resolve: async c =>
                {
                    if (c.Source.ParticipantCounts == null)
                    {
                        c.Source.ParticipantCounts = await c.GetUserContext().Context.CrowdactionParticipantCounts.FindAsync(c.Source.Id).ConfigureAwait(false);
                    }

                    return c.Source.RemainingTimeUserFriendly;
                });
            FieldAsync<NonNullGraphType<BooleanGraphType>, bool>(
                nameof(Crowdaction.IsSuccessfull),
                resolve: async c =>
                {
                    if (c.Source.ParticipantCounts == null)
                    {
                        c.Source.ParticipantCounts = await c.GetUserContext().Context.CrowdactionParticipantCounts.FindAsync(c.Source.Id).ConfigureAwait(false);
                    }

                    return c.Source.IsSuccessfull;
                });
            FieldAsync<NonNullGraphType<BooleanGraphType>, bool>(
                nameof(Crowdaction.IsFailed),
                resolve: async c =>
                {
                    if (c.Source.ParticipantCounts == null)
                    {
                        c.Source.ParticipantCounts = await c.GetUserContext().Context.CrowdactionParticipantCounts.FindAsync(c.Source.Id).ConfigureAwait(false);
                    }

                    return c.Source.IsFailed;
                });
            FieldAsync<NonNullGraphType<IntGraphType>, int>(
                nameof(Crowdaction.TotalParticipants),
                resolve: async c =>
                {
                    if (c.Source.ParticipantCounts == null)
                    {
                        c.Source.ParticipantCounts = await c.GetUserContext().Context.CrowdactionParticipantCounts.FindAsync(c.Source.Id).ConfigureAwait(false);
                    }

                    return c.Source.TotalParticipants;
                });
            FieldAsync<NonNullGraphType<IntGraphType>, int>(
                nameof(Crowdaction.Percentage),
                resolve: async c =>
                {
                    if (c.Source.ParticipantCounts == null)
                    {
                        c.Source.ParticipantCounts = await c.GetUserContext().Context.CrowdactionParticipantCounts.FindAsync(c.Source.Id).ConfigureAwait(false);
                    }

                    return c.Source.Percentage;
                });
            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<InstagramWallItemGraph>>>, IEnumerable<InstagramWallItem>>(
                "instagramWall",
                resolve: c =>
                {
                    string? instagramUser = c.Source.InstagramUser;
                    if (instagramUser != null)
                    {
                        return c.GetUserContext()
                                .ServiceProvider
                                .GetRequiredService<IInstagramService>()
                                .GetItems(instagramUser, c.CancellationToken);
                    }
                    else
                    {
                        return Task.FromResult(Enumerable.Empty<InstagramWallItem>());
                    }
                });
            AddNavigationField(nameof(Crowdaction.DescriptiveImage), c => c.Source.DescriptiveImage);
            AddNavigationField(nameof(Crowdaction.BannerImage), c => c.Source.BannerImage);
            AddNavigationField(nameof(Crowdaction.CardImage), c => c.Source.CardImage);
            AddNavigationField(nameof(Crowdaction.ParticipantCounts), c => c.Source.ParticipantCounts);
            AddNavigationField(nameof(Crowdaction.Owner), c => c.Source.Owner, typeof(RestrictedApplicationUserGraph));
            AddNavigationListField(nameof(Crowdaction.Categories), c => c.Source.Categories);
            AddNavigationListField(nameof(Crowdaction.Participants), c => c.Source.Participants).AuthorizeWith(AuthorizationConstants.GraphQlAdminPolicy);
            AddNavigationListField(nameof(Crowdaction.Tags), c => c.Source.Tags);
            AddNavigationListField(nameof(Crowdaction.Comments), c => c.Source.Comments);
        }
    }
}
