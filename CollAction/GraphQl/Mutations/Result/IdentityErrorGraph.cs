using GraphQL.Types;
using Microsoft.AspNetCore.Identity;

namespace CollAction.GraphQl.Mutations
{
    public class IdentityErrorGraph : ObjectGraphType<IdentityError>
    {
        public IdentityErrorGraph()
        {
            Field(x => x.Code);
            Field(x => x.Description);
        }
    }
}
