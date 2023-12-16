using System;
using System.Collections.Generic;
using FluentResults;
using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Application.InventoryItems;

public class InventoryItemAggregate
{
    private int _sequenceNumber;
    private readonly List<InventoryItemDomainEvent> _domainEvents;

    public InventoryItemAggregate()
    {
        _domainEvents = new List<InventoryItemDomainEvent>();
    }
    
    public string Sku { get; private set; }
    
    public string Description { get;  private set; }

    public decimal Quantity { get;  private set; }

    public Result<InventoryItemDomainEvent<InventoryItemAddedData>> InitNew(Guid eventId, string sku, string description, decimal quantity)
    {
        var data = new InventoryItemAddedData(sku, description, quantity);

        var @event = new InventoryItemDomainEvent<InventoryItemAddedData>(
            id: eventId,
            sequenceNumber: _sequenceNumber + 1,
            timestamp: DateTime.Now,
            type: InventoryDomainEventType.ItemAdded,
            data: data);

        var result = Apply(@event);

        if (result.IsFailed)
        {
            return result;
        }
        
        return @event;
    }

    public Result<InventoryItemDomainEvent<InventoryItemQuantityIncreasedData>> AddQuantity(Guid eventId, decimal quantity)
    {
        var @event = new InventoryItemDomainEvent<InventoryItemQuantityIncreasedData>(
            id: eventId,
            sequenceNumber: _sequenceNumber + 1,
            timestamp: DateTime.Now,
            type: InventoryDomainEventType.QuantityIncreased,
            data: new InventoryItemQuantityIncreasedData(quantity));
     
        var result = Apply(@event);

        if (result.IsFailed)
        {
            return result;
        }
        
        return @event;
    }
    
    public Result<InventoryItemDomainEvent<InventoryItemQuantityIncreasedData>> DecreaseQuantity(Guid eventId, decimal quantity)
    {
        var @event = new InventoryItemDomainEvent<InventoryItemQuantityIncreasedData>(
            id: eventId,
            sequenceNumber: _sequenceNumber + 1,
            timestamp: DateTime.Now,
            type: InventoryDomainEventType.QuantityDecreased,
            data: new InventoryItemQuantityIncreasedData(quantity));
     
        var result = Apply(@event);

        if (result.IsFailed)
        {
            return result;
        }
        
        return @event;
    }

    public Result RestoreStateFrom(IEnumerable<InventoryItemDomainEvent> events)
    {
        foreach (var @event in events)
        {
            var result = Apply(@event);

            if (result.IsFailed) return result;
        }
        
        return Result.Ok();
    }
    
    private Result Apply(InventoryItemDomainEvent @event)
    {
        if (@event.SequenceNumber != _sequenceNumber + 1)
        {
            Result.Fail($"Could not restore the state because the event sequence number '{@event.SequenceNumber}' is out of order.");
        }

        _sequenceNumber = @event.SequenceNumber;

        var result =  @event.Type switch
        {
            InventoryDomainEventType.ItemAdded => Create(@event),
            InventoryDomainEventType.QuantityIncreased => IncreaseQuantity(@event),
            InventoryDomainEventType.QuantityDecreased => DecreaseQuantity(@event),
            _ => Result.Fail($"Event type {@event.Type} was out of range of known values")
        };

        if (result.IsSuccess)
        {
            _domainEvents.Add(@event);
        }

        return result;
    }

    private Result Create(InventoryItemDomainEvent @event)
    {
        if (@event.SequenceNumber != 1)
        {
            return Result.Fail("Create event must have sequence number 1");
        }

        if (_domainEvents.Count != 0)
        {
            return Result.Fail($"{Sku} was already initialized before");
        }
        
        if (@event is not InventoryItemDomainEvent<InventoryItemAddedData> eventWithData)
        {
            return Result.Fail($"Could not cast to type {nameof(InventoryItemDomainEvent<InventoryItemAddedData>)}");
        }

        if (eventWithData.Data.Quantity < 0)
        {
            return Result.Fail($"Can't initialize inventory item with negative quantity. It was {eventWithData.Data.Quantity}");
        }

        
        Sku = eventWithData.Data.Sku;
        Description = eventWithData.Data.Description;
        Quantity = eventWithData.Data.Quantity;
        
        return Result.Ok();
    }

    private Result IncreaseQuantity(InventoryItemDomainEvent @event)
    {
        if (@event is not InventoryItemDomainEvent<InventoryItemQuantityIncreasedData> eventWithData)
        {
            return Result.Fail($"Could not cast to type {nameof(InventoryItemDomainEvent<InventoryItemQuantityIncreasedData>)}");
        }
        
        if (eventWithData.Data.Quantity <= 0)
        {
            return Result.Fail($"Increase quantity must be positive. It was {eventWithData.Data.Quantity}");
        }

        Quantity += eventWithData.Data.Quantity;
        
        return Result.Ok();
    }
    
    private Result DecreaseQuantity(InventoryItemDomainEvent @event)
    {
        if (@event is not InventoryItemDomainEvent<InventoryItemQuantityDecreasedData> eventWithData)
        {
            return Result.Fail($"Could not cast to type {nameof(InventoryItemDomainEvent<InventoryItemQuantityDecreasedData>)}");
        }
        
        if (eventWithData.Data.Quantity <= 0)
        {
            return Result.Fail($"Decrease quantity must be positive. It was {eventWithData.Data.Quantity}");
        }

        if (Quantity < eventWithData.Data.Quantity)
        {
            return Result.Fail($"Can't reduce quantity by {eventWithData.Data.Quantity} because currently available quantity is {Quantity}");
        }
        
        Quantity -= eventWithData.Data.Quantity;
        
        return Result.Ok();
    }
}