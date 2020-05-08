using CollAction.Services.Crowdactions.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public sealed class UpdatedCrowdactionInputGraph : InputObjectGraphType<UpdatedCrowdaction>
    {
        public UpdatedCrowdactionInputGraph()
        {
            Field<NonNullGraphType<IdGraphType>>(nameof(UpdatedCrowdaction.Id), resolve: x => x.Source.Id);
            Field(x => x.Name);
            Field(x => x.Categories);
            Field(x => x.Target);
            Field(x => x.Proposal);
            Field(x => x.Description);
            Field(x => x.Goal);
            Field(x => x.CreatorComments, true);
            Field(x => x.Start);
            Field(x => x.End);
            Field(x => x.BannerImageFileId, true);
            Field(x => x.CardImageFileId, true);
            Field(x => x.DescriptiveImageFileId, true);
            Field(x => x.DescriptionVideoLink, true);
            Field(x => x.Tags);
            Field(x => x.DisplayPriority);
            Field(x => x.NumberCrowdactionEmailsSent);
            Field(x => x.Status);
            Field(x => x.OwnerId, true);
        }
    }
}
