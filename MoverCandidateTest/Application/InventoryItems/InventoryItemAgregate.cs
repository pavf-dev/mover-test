using System;
using System.Collections.Generic;
using FluentResults;
using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Application.InventoryItems;

public class InventoryItemAggregate
{
    private int _sequenceNumber;
    private List<InventoryItemDomainEvent> _domainEvents;
    
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

    public Result<InventoryItemDomainEvent<InventoryQuantityIncreased>> AddQuantity(Guid eventId, decimal quantity)
    {
        var @event = new InventoryItemDomainEvent<InventoryQuantityIncreased>(
            id: eventId,
            sequenceNumber: _sequenceNumber + 1,
            timestamp: DateTime.Now,
            type: InventoryDomainEventType.QuantityIncreased,
            data: new InventoryQuantityIncreased(quantity));
     
        var result = Apply(@event);

        if (result.IsFailed)
        {
            return result;
        }
        
        return @event;
    }
    
    public Result Apply(InventoryItemDomainEvent @event)
    {
        if (@event.SequenceNumber != _sequenceNumber + 1)
        {
            throw new ApplicationException();
        }

        _sequenceNumber = @event.SequenceNumber;
        
        switch (@event.Type)
        {
            case InventoryDomainEventType.ItemAdded:
                return Create(@event);

            case InventoryDomainEventType.QuantityIncreased:
                return IncreaseQuantity(@event);

            case InventoryDomainEventType.QuantityReduced:
                break;
            default:
                return Result.Fail($"Event type {@event.Type} was out of range of known values");
        }
        
        _domainEvents.Add(@event);

        return Result.Ok();
    }

    private Result IncreaseQuantity(InventoryItemDomainEvent @event)
    {
        if (@event is not InventoryItemDomainEvent<InventoryQuantityIncreased> eventWithData)
        {
            return Result.Fail($"Could cast to type {nameof(InventoryItemDomainEvent<InventoryQuantityIncreased>)}");
        }
        
        if (eventWithData.Data.Quantity <= 0)
        {
            return Result.Fail($"Increase quantity must be positive. It was {eventWithData.Data.Quantity}");
        }

        Quantity += eventWithData.Data.Quantity;
        
        return Result.Ok();
    }

    private Result Create(InventoryItemDomainEvent @event)
    {
        if (@event.SequenceNumber != 1)
        {
            return Result.Fail("Create event must have sequence number 1");
        }

        if (@event is not InventoryItemDomainEvent<InventoryItemAddedData> eventWithData)
        {
            return Result.Fail($"Could cast to type {nameof(InventoryItemDomainEvent<InventoryItemAddedData>)}");
        }

        Sku = eventWithData.Data.Sku;
        Description = eventWithData.Data.Description;
        Quantity = eventWithData.Data.Quantity;
        
        return Result.Ok();
    }
}