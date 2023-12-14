using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MoverCandidateTest.Application.Events;
using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Infrastructure;

public class InventoryDomainEventsRepository : IInventoryDomainEventsRepository
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly ConcurrentDictionary<string, List<InventoryItemDomainEvent>> _database = new();

    public IReadOnlyDictionary<string, List<InventoryItemDomainEvent>> GetAll()
    {
        return _database.ToDictionary(x => x.Key, x => x.Value);
    }

    public IEnumerable<InventoryItemDomainEvent> GetAll(string partitionKey)
    {
        if (_database.TryGetValue(partitionKey, out var all)) return all;
        
        return Array.Empty<InventoryItemDomainEvent>();
    }

    public async Task<Result> Add(string partitionKey, InventoryItemDomainEvent item)
    {
        if (_database.TryGetValue(partitionKey, out var list))
        {
            await _semaphore.WaitAsync();
            
            try
            {
                if (list.Any(x => x.Id == item.Id))
                {
                    return Result.Fail(new UniqueKeyConstrainViolationErrorResult(nameof(InventoryItemDomainEvent.Id)));
                }

                if (list.Any(x => x.SequenceNumber == item.SequenceNumber))
                {
                    return Result.Fail(new UniqueKeyConstrainViolationErrorResult(nameof(InventoryItemDomainEvent.SequenceNumber)));
                }
                
                list.Add(item);
            }
            finally
            {
                _semaphore.Release();
            }
            
            return Result.Ok();
        }

        var newList = new List<InventoryItemDomainEvent> { item };
        
        if (!_database.TryAdd(partitionKey, newList))
        {
            return Result.Fail(new UniqueKeyConstrainViolationErrorResult(partitionKey)); 
        }
        
        return Result.Ok();
    }
}