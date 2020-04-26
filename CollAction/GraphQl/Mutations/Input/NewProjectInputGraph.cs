using CollAction.Services.Projects.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public sealed class NewProjectInputGraph : InputObjectGraphType<NewProject>
    {
        public NewProjectInputGraph()
        {
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
        }
    }
}
