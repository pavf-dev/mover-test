using System.Collections.Generic;
using System.Linq;
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
        var items = _inventoryItemsService.GetAll();
        
        return Ok(new RequestResult<IEnumerable<InventoryItemDto>>(items));
    }
    
    [HttpPost]
    public ActionResult CreateOrUpdateInventoryItem([FromBody]CreateOrUpdateInventoryItemRequest request)
    {
        var result = _inventoryItemsService.CreateOrUpdate(new InventoryItemDto(Sku: request.Sku, Description: request.Description, request.Quantity));
        
        if (result.IsFailed)
        {
            return BadRequest(new RequestResult(result.Errors.Select(x => x.Message).ToArray()));
        }
        
        return Ok();
    }
    
    [HttpPost("{sku}/reduce/{quantity}")]
    public ActionResult ReduceQuantity([FromRoute]string sku, [FromRoute] decimal quantity)
    {
        return Ok();
    }
}