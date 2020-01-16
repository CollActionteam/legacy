using CollAction.Services.User.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public sealed class UpdatedUserInputGraph : InputObjectGraphType<UpdatedUser>
    {
        public UpdatedUserInputGraph()
        {
            Field(x => x.Id);
            Field(x => x.Email);
            Field(x => x.FirstName, true);
            Field(x => x.LastName, true);
            Field(x => x.IsSubscribedNewsletter);
        }
    }
}
