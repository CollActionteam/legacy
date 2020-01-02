using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl.Queries
{
    public class ProjectCategoryGraph : EfObjectGraphType<ApplicationDbContext, ProjectCategory>
    {
        public ProjectCategoryGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.Category);
            Field(x => x.ProjectId);
            AddNavigationField(nameof(Project), x => x.Source.Project);
        }
    }
}
