using System.Collections.Generic;

namespace MoverCandidateTest.Application.InventoryItems;

public interface IInventoryItemDtoValidator
{
    IEnumerable<string> Validate(InventoryItemDto dto);
}

public class InventoryItemDtoValidator : IInventoryItemDtoValidator
{
    public IEnumerable<string> Validate(InventoryItemDto dto)
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
