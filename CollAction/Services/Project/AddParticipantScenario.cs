namespace CollAction.Services.Project
{
  public enum AddParticipantScenario
    {
        LoggedInAndAdded = 0,
        AnonymousCreatedAndAdded = 1,
        AnonymousAlreadyRegisteredAndAdded = 2,
        AnonymousNotRegisteredPresentAndAdded = 3,
        AnonymousAlreadyRegisteredAndAlreadyParticipating = 4,
        AnonymousNotRegisteredPresentAndAlreadyParticipating = 5
    }
}
