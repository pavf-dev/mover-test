using System.Text.Json.Serialization;

namespace MoverCandidateTest.Controllers.Inventory;

public class CreateOrUpdateInventoryItemRequest
{
    [JsonConstructor]
    public CreateOrUpdateInventoryItemRequest(string sku, string description, decimal quantity)
    {
        Sku = sku;
        Description = description;
        Quantity = quantity;
    }
    
    public string Sku { get; }
    
    public string Description { get; }
    
    public decimal Quantity { get; }
}