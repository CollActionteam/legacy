using CollAction.Services.User.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations.Input
{
    public class NewUserInputGraph : InputObjectGraphType<NewUser>
    {
        public NewUserInputGraph()
        {
            Field(x => x.Email);
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field(x => x.Password);
            Field(x => x.IsSubscribedNewsletter);
        }
    }
}
