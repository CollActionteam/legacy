using System;

namespace CollAction.Services.Projects.Models
{
    public class AddParticipantResult
    {
        public string Error { get; set; }
        public bool LoggedIn { get; set; }
        public bool UserAdded { get; set; }
        public bool UserCreated { get; set; }
        public bool UserAlreadyActive { get; set; }
        public string ParticipantEmail { get; set; }
        public string PasswordResetToken { get; set; }

        public AddParticipantScenario Scenario
        {
            get
            {
                if (Error != null)
                    return AddParticipantScenario.Error;
                else if (LoggedIn && UserAdded)
                    return AddParticipantScenario.LoggedInAndAdded;
                else if (!LoggedIn && UserCreated && UserAdded)
                    return AddParticipantScenario.AnonymousCreatedAndAdded;
                else if (!LoggedIn && UserAlreadyActive && UserAdded)
                    return AddParticipantScenario.AnonymousAlreadyRegisteredAndAdded;
                else if (!LoggedIn && !UserAlreadyActive && UserAdded)
                    return AddParticipantScenario.AnonymousNotRegisteredPresentAndAdded;
                else if (!LoggedIn && UserAlreadyActive && !UserAdded)
                    return AddParticipantScenario.AnonymousAlreadyRegisteredAndAlreadyParticipating;
                else if (!LoggedIn && !UserAlreadyActive && !UserAdded)
                    return AddParticipantScenario.AnonymousNotRegisteredPresentAndAlreadyParticipating;
                else
                    throw new InvalidOperationException(
                        "No participant scenario available for " +
                        $"{nameof(LoggedIn)}:{LoggedIn}, {nameof(UserAdded)}:{UserAdded}, " +
                        $"{nameof(UserCreated)}:{UserCreated}, {nameof(UserAlreadyActive)}:{UserAlreadyActive}");
            }
        }
    }
}