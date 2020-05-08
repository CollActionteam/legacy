using CollAction.Services.Crowdactions.Models;
using GraphQL.Types;

namespace CollAction.GraphQl.Mutations
{
    public sealed class AddParticipantResultGraph : ObjectGraphType<AddParticipantResult>
    {
        public AddParticipantResultGraph()
        {
            Field(x => x.Error, true);
            Field(x => x.LoggedIn);
            Field(x => x.Scenario);
            Field(x => x.UserAdded);
            Field(x => x.UserAlreadyActive);
            Field(x => x.UserCreated);
        }
    }
}
