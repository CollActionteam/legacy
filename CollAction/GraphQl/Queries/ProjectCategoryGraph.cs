using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class ProjectCategoryGraph : EfObjectGraphType<ApplicationDbContext, ProjectCategory>
    {
        public ProjectCategoryGraph(IEfGraphQLService<ApplicationDbContext> graphService) : base(graphService)
        {
            Field(x => x.Category);
            Field<NonNullGraphType<IdGraphType>>(nameof(ProjectCategory.ProjectId), resolve: x => x.Source.ProjectId);
            AddNavigationField(nameof(Project), x => x.Source.Project);
        }
    }
}
