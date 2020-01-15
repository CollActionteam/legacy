using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    public sealed class ProjectCategoryGraph : EfObjectGraphType<ApplicationDbContext, ProjectCategory>
    {
        public ProjectCategoryGraph(IEfGraphQLService<ApplicationDbContext> graphService) : base(graphService)
        {
            Field(x => x.Category);
            Field(x => x.ProjectId);
            AddNavigationField(nameof(Project), x => x.Source.Project);
        }
    }
}
