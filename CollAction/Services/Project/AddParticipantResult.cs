using System;

namespace CollAction.Services.Project
{
  public class AddParticipantResult
    {
        public bool LoggedIn { get; set;}
        public bool UserAdded {get; set;}
        public bool UserCreated { get; set; }
        public bool UserAlreadyActive { get; set; }
        public string ParticipantEmail { get; set;}
        public string PasswordResetToken { get; set; }

        public AddParticipantScenario Scenario 
        { 
            get 
            {
                if (LoggedIn && UserAdded) return AddParticipantScenario.LoggedInAndAdded;
                if (!LoggedIn && UserCreated && UserAdded) return AddParticipantScenario.AnonymousCreatedAndAdded;
                if (!LoggedIn && UserAlreadyActive && UserAdded) return AddParticipantScenario.AnonymousAlreadyRegisteredAndAdded;
                if (!LoggedIn && !UserAlreadyActive && UserAdded) return AddParticipantScenario.AnonymousNotRegisteredPresentAndAdded;
                if (!LoggedIn && UserAlreadyActive && !UserAdded) return AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating;
                if (!LoggedIn && !UserAlreadyActive && !UserAdded) return AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating;

                throw new InvalidOperationException(
                    "No participant scenario available for " +
                    $"{nameof(LoggedIn)}:{LoggedIn}, {nameof(UserAdded)}:{UserAdded}, " +
                    $"{nameof(UserCreated)}:{UserCreated}, {nameof(UserAlreadyActive)}:{UserAlreadyActive}");
            }
        }
    }
}
