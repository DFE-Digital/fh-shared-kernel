using FamilyHubs.SharedKernel;

namespace FamilyHubs.SharedKernel.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<EntityBase<string>> entitiesWithEvents);
}
