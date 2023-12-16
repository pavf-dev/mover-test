namespace MoverCandidateTest.Domain;

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