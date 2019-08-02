using GraphQL.Types;

namespace CollAction.GraphQl.Mutations
{
    public class MutationGraph : ObjectGraphType
    {
        public MutationGraph()
        {
            Field<ApplicationUserMutationGraph>(
                "applicationUser",
                resolve: c => new object());

            Field<DonationMutationGraph>(
                "donation",
                resolve: c => new object());

            Field<ProjectMutationGraph>(
                "project",
                resolve: c => new object());
        }
    }
}