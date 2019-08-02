using CollAction.Services.User.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public class UpdatedUserInputGraph : InputObjectGraphType<UpdatedUser>
    {
        public UpdatedUserInputGraph()
        {
            Field(x => x.Id);
            Field(x => x.Email);
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field(x => x.IsSubscribedNewsletter);
        }
    }
}
