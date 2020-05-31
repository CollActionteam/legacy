using CollAction.Services.User.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public sealed class NewUserInputGraph : InputObjectGraphType<NewUser>
    {
        public NewUserInputGraph()
        {
            Field(x => x.Email);
            Field(x => x.FirstName, true);
            Field(x => x.LastName, true);
            Field(x => x.Password);
            Field(x => x.IsSubscribedNewsletter);
        }
    }
}
