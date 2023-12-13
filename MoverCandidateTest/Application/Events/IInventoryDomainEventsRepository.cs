using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Application.Events;

public interface IInventoryDomainEventsRepository : IEventsRepository<InventoryItemDomainEvent> { }