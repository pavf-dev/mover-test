namespace MoverCandidateTest.Domain;

public class InventoryItemQuantityIncreasedData
{
    public InventoryItemQuantityIncreasedData(decimal quantity)
    {
        Quantity = quantity;
    }

    public decimal Quantity { get; }
}