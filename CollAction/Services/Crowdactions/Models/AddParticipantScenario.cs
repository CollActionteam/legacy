namespace CollAction.Services.Crowdactions.Models
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
