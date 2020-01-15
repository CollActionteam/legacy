using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    public sealed class ProjectTagGraph : EfObjectGraphType<ApplicationDbContext, ProjectTag>
    {
        public ProjectTagGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field(x => x.ProjectId);
            Field(x => x.TagId);
            AddNavigationField(nameof(ProjectTag.Project), c => c.Source.Project);
            AddNavigationField(nameof(ProjectTag.Tag), c => c.Source.Tag);
        }
    }
}
