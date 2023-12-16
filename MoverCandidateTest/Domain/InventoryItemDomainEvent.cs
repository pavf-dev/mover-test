using System;

namespace MoverCandidateTest.Domain;

public class InventoryItemDomainEvent<T> : InventoryItemDomainEvent
{
    public InventoryItemDomainEvent(Guid id, int sequenceNumber, DateTime timestamp, InventoryDomainEventType type,
        T data) : base(id, sequenceNumber, timestamp, type)
    {
        Data = data;
    }
    
    public T Data { get; }
}

public class InventoryItemDomainEvent
{
    public InventoryItemDomainEvent(Guid id, int sequenceNumber, DateTime timestamp, InventoryDomainEventType type)
    {
        Id = id;
        SequenceNumber = sequenceNumber;
        Timestamp = timestamp;
        Type = type;
    }
    
    public Guid Id { get; }
    
    public int SequenceNumber { get; }
    
    public DateTime Timestamp { get; }
    
    public InventoryDomainEventType Type { get; } 
}