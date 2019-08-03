using CollAction.Data;
using CollAction.Models;
using CollAction.Services.Projects;
using GraphQL.Authorization;
using GraphQL.EntityFramework;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.GraphQl.Queries
{
    public class ProjectGraph : EfObjectGraphType<ApplicationDbContext, Project>
    {
        public ProjectGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService, IServiceScopeFactory serviceScopeFactory) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.AnonymousUserParticipants);
            Field(x => x.BannerImageFileId, true);
            Field(x => x.CategoryId);
            Field(x => x.CreatorComments, true);
            Field(x => x.Description);
            Field(x => x.DescriptionVideoLink, true);
            Field(x => x.DescriptiveImageFileId, true);
            Field(x => x.DisplayPriority);
            Field(x => x.End);
            Field(x => x.Goal);
            Field(x => x.IsActive);
            Field(x => x.IsClosed);
            Field(x => x.IsComingSoon);
            Field(x => x.Name);
            Field(x => x.NumberProjectEmailsSend);
            Field(x => x.OwnerId);
            Field(x => x.Proposal);
            Field(x => x.RemainingTime);
            Field(x => x.Start);
            Field(x => x.Status);
            Field(x => x.Target);
            Field(x => x.NameNormalized);
            Field(x => x.Url);
            Field<BooleanGraphType>(
                "canSendProjectEmail",
                resolve: c =>
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return scope.ServiceProvider.GetRequiredService<IProjectService>().CanSendProjectEmail(c.Source);
                    }
                });
            AddNavigationField(nameof(Project.Category), c => c.Source.Category);
            AddNavigationField(nameof(Project.DescriptiveImage), c => c.Source.DescriptiveImage);
            AddNavigationField(nameof(Project.BannerImage), c => c.Source.BannerImage);
            AddNavigationField(nameof(Project.ParticipantCounts), c => c.Source.ParticipantCounts);
            AddNavigationField(nameof(Project.Owner), c => c.Source.Owner).AuthorizeWith(Constants.GraphQlAdminPolicy);
            AddNavigationListField(nameof(Project.Participants), c => c.Source.Participants).AuthorizeWith(Constants.GraphQlAdminPolicy);
            AddNavigationListField(nameof(Project.Tags), c => c.Source.Tags);
        }
    }
}
