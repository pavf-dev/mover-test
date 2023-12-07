using System.Collections.Generic;
using System.Linq;
using FluentResults;
using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Application.InventoryItems;

public class InventoryItemsService
{
    private readonly IInventoryItemsRepository _repository;
    
    public InventoryItemsService(IInventoryItemsRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<InventoryItemDto> GetAll()
    {
        var allItems = _repository.GetAll();

        return allItems
            .Select(x => new InventoryItemDto(Sku: x.Sku, Description: x.Description, x.Quantity))
            .ToArray();
    }

    public Result CreateOrUpdate(InventoryItemDto inventoryItemDto)
    {
        var validationResult = Validate(inventoryItemDto);

        if (validationResult.Count > 0) return Result.Fail(validationResult);
        
        var existingInventoryItem = _repository.Get(inventoryItemDto.Sku);

        if (existingInventoryItem is not null)
        {
            var updatedItemDto = existingInventoryItem with
            {
                Description = inventoryItemDto.Description,
                Quantity = existingInventoryItem.Quantity + inventoryItemDto.Quantity
            };
            
            _repository.Update(updatedItemDto);
            
            return Result.Ok();
        }

        var newInventoryItem = new InventoryItem(Sku: inventoryItemDto.Sku, Description: inventoryItemDto.Description,
            inventoryItemDto.Quantity);
        
        _repository.Add(newInventoryItem);
        
        return Result.Ok();
    }

    private List<string> Validate(InventoryItemDto dto)
    {
        var validationErrors = new List<string>();
        
        if (string.IsNullOrEmpty(dto.Sku))
        {
            validationErrors.Add("Sku must not be empty");
        }

        if (string.IsNullOrEmpty(dto.Description))
        {
            validationErrors.Add("Description must not be empty");
        }

        if (dto.Quantity < 0)
        {
            validationErrors.Add("Quantity must be equal to or greater than 0");
        }

        return validationErrors;
    }
}