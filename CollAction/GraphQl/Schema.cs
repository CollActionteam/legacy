using CollAction.GraphQl.Mutations;
using CollAction.GraphQl.Queries;
using GraphQL;

namespace CollAction.GraphQl
{
    public class Schema : GraphQL.Types.Schema
    {
        public Schema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<Query>();
            Mutation = resolver.Resolve<Mutation>();
        }
    }
}
