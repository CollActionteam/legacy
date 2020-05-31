using CollAction.GraphQl.Queries;
using CollAction.Models;
using CollAction.Services.User.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations
{
    public sealed class UserResultGraph : ObjectGraphType<UserResult>
    {
        public UserResultGraph()
        {
            Field<ApplicationUserGraph, ApplicationUser?>(nameof(UserResult.User)).Resolve(c => c.Source.User);
            Field(x => x.Result);
        }
    }
}
