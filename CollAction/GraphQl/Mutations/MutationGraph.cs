using GraphQL.Types;

namespace CollAction.GraphQl.Mutations
{
    public sealed class MutationGraph : ObjectGraphType
    {
        public MutationGraph()
        {
            Field<UserMutationGraph>(
                "user",
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