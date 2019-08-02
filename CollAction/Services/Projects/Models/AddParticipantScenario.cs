namespace CollAction.Services.Projects.Models
{
    public enum AddParticipantScenario
    {
        LoggedInAndAdded,
        AnonymousCreatedAndAdded,
        AnonymousAlreadyRegisteredAndAdded,
        AnonymousNotRegisteredPresentAndAdded,
        AnonymousAlreadyRegisteredAndAlreadyParticipating,
        AnonymousNotRegisteredPresentAndAlreadyParticipating,
        Error
    }
}
