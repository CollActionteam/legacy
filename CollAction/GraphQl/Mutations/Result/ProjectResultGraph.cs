using CollAction.Services.Projects.Models;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.GraphQl.Mutations.Result
{
    public sealed class ProjectResultGraph : ObjectGraphType<ProjectResult>
    {
        public ProjectResultGraph()
        {
            Field(x => x.Project);
            Field(x => x.Succeeded);
            Field(x => x.Errors);
        }
    }
}
