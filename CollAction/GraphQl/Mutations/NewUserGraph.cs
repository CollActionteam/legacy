using GraphQL.Types;

namespace CollAction.GraphQl.Mutations
{
    public class NewUserGraph : InputObjectGraphType<NewUser>
    {
        public NewUserGraph()
        {
            Field(x => x.Email);
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field(x => x.Password);
            Field(x => x.SubscribeNewsletter);
        }
    }
}
