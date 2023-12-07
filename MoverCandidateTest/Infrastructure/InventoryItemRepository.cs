using System.Collections.Generic;
using System.Linq;
using MoverCandidateTest.Application.InventoryItems;
using MoverCandidateTest.Domain;

namespace MoverCandidateTest.Infrastructure;

public class InventoryItemsRepository : IInventoryItemsRepository
{
    private readonly Dictionary<string, InventoryItem> _items;

    public InventoryItemsRepository()
    {
        var delorean = new InventoryItem("delorean", "it's a time machine", 11);
        var ferrari = new InventoryItem("ferrari", "nah. former champions", 1);
        _items = new Dictionary<string, InventoryItem>
        {
            { delorean.Sku, delorean },
            { ferrari.Sku, ferrari }
        };
    }
    
    public IEnumerable<InventoryItem> GetAll()
    {
        return _items.Select(x => x.Value);
    }

    public void Add(InventoryItem item)
    {
        var skuLowerCase = item.Sku.ToLower();

        if (_items.ContainsKey(skuLowerCase)) return;
        
        _items.Add(skuLowerCase, item);
    }

    public InventoryItem Get(string id)
    {
        var skuLowerCase = id.ToLower();

        return _items.TryGetValue(skuLowerCase, out InventoryItem value) ? value : null;
    }

    public void Update(InventoryItem item)
    {
        var skuLowerCase = item.Sku.ToLower();
        
        if (!_items.ContainsKey(skuLowerCase)) return;

        _items[skuLowerCase] = item;
    }
}