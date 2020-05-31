using CollAction.GraphQl.Mutations;
using CollAction.GraphQl.Queries;
using GraphQL;

namespace CollAction.GraphQl
{
    public sealed class GraphQlSchema : GraphQL.Types.Schema
    {
        public GraphQlSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<QueryGraph>();
            Mutation = resolver.Resolve<MutationGraph>();
        }
    }
}
