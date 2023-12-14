using FluentResults;

namespace MoverCandidateTest.Application.InventoryItems;

public partial class InventoryItemsService
{
    public class ValidationError : Error
    {
        public ValidationError(string message) : base(message)
        {
        }
    }
    
    public class InventoryItemUpdateConflict : Error
    {
    }

    public class RequestIsAlreadyProcessed : Error
    {
    }
}