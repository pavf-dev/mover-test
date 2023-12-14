using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MoverCandidateTest.Application.Events;
using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Application.InventoryItems;

public partial class InventoryItemsService
{
    private readonly IInventoryDomainEventsRepository _inventoryEventsRepository;
    
    public InventoryItemsService(IInventoryDomainEventsRepository inventoryEventsRepository)
    {
        _inventoryEventsRepository = inventoryEventsRepository;
    }

    public Result<IEnumerable<InventoryItemDto>> GetAll()
    {
        var allEvents = _inventoryEventsRepository.GetAll();
        var allInventoryItem = new List<InventoryItemDto>(allEvents.Keys.Count());
        var errors = new List<string>();
        
        foreach (var sku in allEvents.Keys)
        {
            var inventoryItem = new InventoryItemAggregate();
            var result = inventoryItem.Apply(allEvents[sku]);
            
            if (result.IsFailed)
            {
                errors.Add($"Could not restore the state for {sku}"); 
            }
            else
            {
                allInventoryItem.Add(new InventoryItemDto(Sku: inventoryItem.Sku, Description: inventoryItem.Description, inventoryItem.Quantity));
            }
        }
        
        if (errors.Any()) return Result.Fail(errors);
        
        return Result.Ok((IEnumerable<InventoryItemDto>)allInventoryItem);
    }

    public async Task<Result> CreateOrUpdate(InventoryItemDto inventoryItemDto, Guid eventId)
    {
        var validationResult = Validate(inventoryItemDto);

        if (validationResult.IsFailed) return validationResult;

        var inventoryEvents = _inventoryEventsRepository.GetAll(inventoryItemDto.Sku).ToArray();
        var inventoryItem = new InventoryItemAggregate();
        
        if (inventoryEvents.Any())
        {
            var result = inventoryItem.Apply(inventoryEvents);

            if (result.IsFailed)
            {
                return Result.Fail($"Could not restore the state for {inventoryItemDto.Sku}"); 
            }

            var addQuantityResult = inventoryItem.AddQuantity(eventId, inventoryItemDto.Quantity);

            if (addQuantityResult.IsFailed)
            {
                return Result.Fail(addQuantityResult.Errors);
            }
            
            return await SaveEvent(inventoryItem.Sku, addQuantityResult.Value);
        }

        var initResult = inventoryItem.InitNew(
            eventId: eventId,
            sku: inventoryItemDto.Sku,
            description: inventoryItemDto.Description,
            quantity: inventoryItemDto.Quantity);
        
        if (initResult.IsFailed)
        {
            return Result.Fail(initResult.Errors);
        }

        return await SaveEvent(inventoryItem.Sku, initResult.Value);
    }

    private async Task<Result> SaveEvent(string partitionKey, InventoryItemDomainEvent @event)
    {
        var saveResult = await _inventoryEventsRepository.Add(partitionKey, @event);

        if (saveResult.IsSuccess) return Result.Ok();
        
        if (saveResult.HasError<UniqueKeyConstrainViolationErrorResult>())
        {
            var error = saveResult.Errors.Single() as UniqueKeyConstrainViolationErrorResult;

            var returnResult = error.PropertyName switch
            {
                nameof(InventoryItemDomainEvent.Id) => new RequestIsAlreadyProcessed(),
                nameof(InventoryItemDomainEvent.SequenceNumber) => new InventoryItemUpdateConflict(),
                _ => new Error($"Unique of {error.PropertyName} is violated")
            };

            return Result.Fail(returnResult);
        }

        return Result.Fail(saveResult.Errors);
    }
    
    private Result Validate(InventoryItemDto dto)
    {
        var result = new Result();
        
        if (string.IsNullOrEmpty(dto.Sku))
        {
            result.WithError(new ValidationError("Sku must not be empty"));
        }

        if (string.IsNullOrEmpty(dto.Description))
        {
            result.WithError(new ValidationError("Description must not be empty"));
        }

        if (dto.Quantity < 0)
        {
            result.WithError(new ValidationError("Quantity must be equal to or greater than 0"));
        }

        return result;
    }
}