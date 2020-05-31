using GraphQL.Types;

namespace CollAction.GraphQl.Mutations
{
    public sealed class MutationGraph : ObjectGraphType
    {
        public MutationGraph()
        {
            Field<NonNullGraphType<UserMutationGraph>>(
                "user",
                resolve: c => new object());

            Field<NonNullGraphType<DonationMutationGraph>>(
                "donation",
                resolve: c => new object());

            Field<NonNullGraphType<CrowdactionMutationGraph>>(
                "crowdaction",
                resolve: c => new object());
        }
    }
}