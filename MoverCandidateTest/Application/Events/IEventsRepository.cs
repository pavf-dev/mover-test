using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;

namespace MoverCandidateTest.Application.Events;

public interface IEventsRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    IEnumerable<T> GetAll(string partitionKey);
    Task<Result> Add(string partitionKey, T item);
}