using CollAction.Data;
using CollAction.Models;
using GraphQL.EntityFramework;

namespace CollAction.GraphQl
{
    public class ApplicationUserGraph : EfObjectGraphType<ApplicationDbContext, ApplicationUser>
    {
        public ApplicationUserGraph(IEfGraphQLService<ApplicationDbContext> efGraphQlService) : base(efGraphQlService)
        {
            Field(x => x.Id);
            Field(x => x.Email);
            Field(x => x.EmailConfirmed);
            Field(x => x.FirstName);
            Field(x => x.FullName);
            Field(x => x.LastName);
            AddNavigationListField(nameof(ApplicationUser.Projects), c => c.Source.Projects);
        }
    }
}
