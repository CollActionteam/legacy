using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;
using GraphQL.Types;

namespace CollAction.GraphQl.Queries
{
    public sealed class ProjectTagGraph : EfObjectGraphType<ApplicationDbContext, ProjectTag>
    {
        public ProjectTagGraph(IEfGraphQLService<ApplicationDbContext> entityFrameworkGraphQlService) : base(entityFrameworkGraphQlService)
        {
            Field<NonNullGraphType<IdGraphType>>(nameof(ProjectTag.ProjectId), resolve: x => x.Source.ProjectId);
            Field<NonNullGraphType<IdGraphType>>(nameof(ProjectTag.TagId), resolve: x => x.Source.TagId);
            AddNavigationField(nameof(ProjectTag.Project), c => c.Source.Project);
            AddNavigationField(nameof(ProjectTag.Tag), c => c.Source.Tag);
        }
    }
}
