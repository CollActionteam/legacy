using CollAction.Services.Projects.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Result
{
    public sealed class ProjectResultGraph : ObjectGraphType<ProjectResult>
    {
        public ProjectResultGraph()
        {
            Field(x => x.Project, true);
            Field(x => x.Succeeded);
            Field(x => x.Errors);
        }
    }
}
