using CollAction.Services.Projects.Models;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.GraphQl.Mutations
{
    public class AddParticipantResultGraph : ObjectGraphType<AddParticipantResult>
    {
        public AddParticipantResultGraph()
        {
            Field(x => x.Error);
            Field(x => x.LoggedIn);
            Field(x => x.ParticipantEmail);
            Field(x => x.PasswordResetToken);
            Field(x => x.Scenario);
            Field(x => x.UserAdded);
            Field(x => x.UserAlreadyActive);
            Field(x => x.UserCreated);
        }
    }
}
