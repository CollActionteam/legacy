using CollAction.Services.Projects.Models;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CollAction.GraphQl.Mutations.Input
{
    public class NewProjectInputGraph : InputObjectGraphType<NewProject>
    {
        public NewProjectInputGraph()
        {
            Field(x => x.Name);

            Field(x => x.CategoryId);

            Field(x => x.Target);

            Field(x => x.Proposal);


            Field(x => x.Description);

            Field(x => x.Goal);

            Field(x => x.CreatorComments);

            Field(x => x.Start);

            Field(x => x.End);

            Field(x => x.BannerImageFileId, true);

            Field(x => x.DescriptiveImageFileId, true);

            Field(x => x.DescriptionVideoLink);

            Field(x => x.Tags);
        }
    }
}
