using System.Collections.Generic;
using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Application.InventoryItems;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    void Add(T item);
    T Get(string id);
    void Update(T item);
}

public interface IInventoryItemsRepository : IRepository<InventoryItem> { }



