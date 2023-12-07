using System;

namespace MoverCandidateTest.Controllers.Inventory;

public class RequestResult
{
    public RequestResult()
    {
        Errors = Array.Empty<string>();
    }
    
    public RequestResult(string[] errors)
    {
        Errors = errors;
    }
    
    public string[] Errors { get; }
}

public class RequestResult<T> : RequestResult
{
    public RequestResult(T data)
    {
        Data = data;
    }

    public T Data { get; }
}