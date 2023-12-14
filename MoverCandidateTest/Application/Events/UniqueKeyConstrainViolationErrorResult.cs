using FluentResults;

namespace MoverCandidateTest.Application.Events;

public class UniqueKeyConstrainViolationErrorResult : Error
{
    public UniqueKeyConstrainViolationErrorResult(string propertyName)
    {
        PropertyName = propertyName;
    }
    
    public string PropertyName { get; }
}