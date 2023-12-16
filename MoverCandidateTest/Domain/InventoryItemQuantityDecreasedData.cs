namespace MoverCandidateTest.Domain;

public class InventoryItemQuantityDecreasedData
{
    public InventoryItemQuantityDecreasedData(decimal quantity)
    {
        Quantity = quantity;
    }

    public decimal Quantity { get; }
}