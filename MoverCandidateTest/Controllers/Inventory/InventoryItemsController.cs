using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoverCandidateTest.Application.InventoryItems;

namespace MoverCandidateTest.Controllers.Inventory;

[ApiController]
[Route("inventory-items")]
public class InventoryItemsController : ControllerBase
{
    private readonly InventoryItemsService _inventoryItemsService;

    public InventoryItemsController(InventoryItemsService inventoryItemsService)
    {
        _inventoryItemsService = inventoryItemsService;
    }
    
    [HttpGet]
    public ActionResult<RequestResult<IEnumerable<InventoryItemDto>>> ListInventoryItems()
    {
        var getAllResult = _inventoryItemsService.GetAll();

        if (getAllResult.IsFailed) return StatusCode(500, new RequestResult(getAllResult.Errors.Select(s => s.Message)));

        return Ok(new RequestResult<IEnumerable<InventoryItemDto>>(getAllResult.Value));
    }
    
    [HttpPut]
    public async Task<ActionResult> CreateOrUpdateInventoryItem(
        [Required][FromHeader(Name = "Idempotency-Key")] Guid idempotencyKey,
        [FromBody] CreateOrUpdateInventoryItemRequest request)
    {
        var result = await _inventoryItemsService.CreateOrUpdate(
            new InventoryItemDto(Sku: request.Sku, Description: request.Description, request.Quantity), idempotencyKey);

        if (result.IsSuccess) return NoContent();

        if (result.HasError<InventoryItemsService.ValidationError>())
        {
            return UnprocessableEntity(new RequestResult(result.Errors.Select(x => x.Message)));
        }
        
        if (result.HasError<InventoryItemsService.RequestIsAlreadyProcessed>() ||
            result.HasError<InventoryItemsService.InventoryItemUpdateConflict>())
        {
            return Conflict();
        }
            
        return StatusCode(500, new RequestResult(result.Errors.Select(s => s.Message)));
    }
    
    [HttpPut("{sku}/decrease/{quantity}")]
    public ActionResult DecreaseQuantity(
        [Required][FromHeader(Name = "Idempotency-Key")] Guid idempotencyKey,
        [FromRoute] string sku,
        [FromRoute] decimal quantity)
    {
        return Ok();
    }
}