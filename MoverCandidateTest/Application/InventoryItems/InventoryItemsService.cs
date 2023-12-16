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
    private readonly IInventoryItemDtoValidator _dtoValidator;

    public InventoryItemsService(IInventoryDomainEventsRepository inventoryEventsRepository,
        IInventoryItemDtoValidator dtoValidator)
    {
        _inventoryEventsRepository = inventoryEventsRepository;
        _dtoValidator = dtoValidator;
    }

    public Result<IEnumerable<InventoryItemDto>> GetAll()
    {
        var allEvents = _inventoryEventsRepository.GetAll();
        var allInventoryItem = new List<InventoryItemDto>(allEvents.Keys.Count());
        var errors = new List<string>();
        
        foreach (var sku in allEvents.Keys)
        {
            var inventoryItem = new InventoryItemAggregate();
            var result = inventoryItem.RestoreStateFrom(allEvents[sku]);
            
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
        var validationResult = _dtoValidator.Validate(inventoryItemDto).ToList();

        if (validationResult.Any()) return Result.Fail(validationResult.Select(x => new ValidationError(x)));

        var inventoryEvents = _inventoryEventsRepository.GetAll(inventoryItemDto.Sku).ToArray();
        var inventoryItem = new InventoryItemAggregate();
        
        if (inventoryEvents.Any())
        {
            var result = inventoryItem.RestoreStateFrom(inventoryEvents);

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

        if (!saveResult.HasError<UniqueKeyConstrainViolationErrorResult>())
        {
            return Result.Fail(saveResult.Errors);
        }
        
        var error = saveResult.Errors.Single() as UniqueKeyConstrainViolationErrorResult;

        var returnResult = error.PropertyName switch
        {
            nameof(InventoryItemDomainEvent.Id) => new RequestIsAlreadyProcessed(),
            nameof(InventoryItemDomainEvent.SequenceNumber) => new InventoryItemUpdateConflict(),
            _ => new Error($"Unique of {error.PropertyName} is violated")
        };

        return Result.Fail(returnResult);
    }
}