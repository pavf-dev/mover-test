using System;
using System.Collections.Generic;

namespace MoverCandidateTest.Controllers.Inventory;

public class RequestResult
{
    public RequestResult()
    {
        Errors = Array.Empty<string>();
    }
    
    public RequestResult(IEnumerable<string> errors)
    {
        Errors = errors;
    }
    
    public IEnumerable<string> Errors { get; }
}

public class RequestResult<T> : RequestResult
{
    public RequestResult(T data)
    {
        Data = data;
    }

    public T Data { get; }
}