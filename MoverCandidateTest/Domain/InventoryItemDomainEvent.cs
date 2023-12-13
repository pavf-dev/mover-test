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

public enum InventoryDomainEventType
{
    ItemAdded = 1,
    QuantityIncreased,
    QuantityReduced
}

public class InventoryItemAddedData
{
    public InventoryItemAddedData(string sku, string description, decimal quantity)
    {
        Sku = sku;
        Description = description;
        Quantity = quantity;
    }

    public string Sku { get; }

    public string Description { get; }

    public decimal Quantity { get; }
}

public class InventoryQuantityIncreased
{
    public InventoryQuantityIncreased(decimal quantity)
    {
        Quantity = quantity;
    }

    public decimal Quantity { get; }
}