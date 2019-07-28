using GraphQL;

namespace CollAction.GraphQl
{
    public class Schema : GraphQL.Types.Schema
    {
        public Schema(IDependencyResolver resolver) :
        base(resolver)
        {
            Query = resolver.Resolve<Query>();
        }
    }
}
