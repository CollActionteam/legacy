namespace CollAction.Services.Projects.Models
{
    public enum AddParticipantScenario
    {
        LoggedInAndAdded,
        LoggedInAndNotAdded,
        AnonymousCreatedAndAdded,
        AnonymousAlreadyRegisteredAndAdded,
        AnonymousNotRegisteredPresentAndAdded,
        AnonymousAlreadyRegisteredAndAlreadyParticipating,
        AnonymousNotRegisteredPresentAndAlreadyParticipating,
        Error
    }
}
