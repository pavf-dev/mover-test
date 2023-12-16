using System;
using MoverCandidateTest.Application.InventoryItems;
using MoverCandidateTest.Domain;
using NUnit.Framework;

namespace WatchHandsCalculatorsTests.InventoryLogicTests;

public class InventoryItemAggregateTests
{
    [Test]
    public void Initialization_of_new_item_returns_valid_domain_event()
    {
        var eventId = Guid.NewGuid();
        var sku = "this is sku";
        var description = "meaningful description";
        var quantity = 12.54m;
        
        var aggregate = new InventoryItemAggregate();

        // Act
        var result = aggregate.InitNew(eventId, sku, description, quantity);
        
        Assert.IsTrue(result.IsSuccess);
        var inventoryEvent = result.Value;
        Assert.AreEqual(1, inventoryEvent.SequenceNumber);
        Assert.AreEqual(InventoryDomainEventType.ItemAdded, inventoryEvent.Type);
        Assert.AreEqual(eventId, inventoryEvent.Id);
        Assert.AreEqual(sku, inventoryEvent.Data.Sku);
        Assert.AreEqual(description, inventoryEvent.Data.Description);
        Assert.AreEqual(quantity, inventoryEvent.Data.Quantity);
    }

    [Test]
    public void Initialization_puts_aggregate_into_right_state()
    {
        var eventId = Guid.NewGuid();
        var sku = "this is sku";
        var description = "meaningful description";
        var quantity = 12.54m;
        
        var aggregate = new InventoryItemAggregate();

        // Act
        aggregate.InitNew(eventId, sku, description, quantity);
        
        Assert.AreEqual(sku, aggregate.Sku);
        Assert.AreEqual(description, aggregate.Description);
        Assert.AreEqual(quantity, aggregate.Quantity);
    }

    [Test]
    public void Aggregate_restores_itself_from_ItemAdded_domain_event()
    {
        var eventId = Guid.NewGuid();
        var sku = "this is sku";
        var description = "meaningful description";
        var quantity = 12.54m;

        var domainEvent = new InventoryItemDomainEvent<InventoryItemAddedData>(eventId, 1, DateTime.Now,
            InventoryDomainEventType.ItemAdded, new InventoryItemAddedData(sku, description, quantity));
        
        var aggregate = new InventoryItemAggregate();

        // Act
        var result = aggregate.RestoreStateFrom(new []{ domainEvent });
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(sku, aggregate.Sku);
        Assert.AreEqual(description, aggregate.Description);
        Assert.AreEqual(quantity, aggregate.Quantity);
    }

    [Test]
    public void Aggregate_restoration_fails_on_out_of_order_sequence_number()
    {
        var domainEventItemAdded = new InventoryItemDomainEvent<InventoryItemAddedData>(
            Guid.NewGuid(), sequenceNumber: 1, DateTime.Now, InventoryDomainEventType.ItemAdded, new InventoryItemAddedData("something", string.Empty, 1));
        var domainEventItemQuantityIncreased = new InventoryItemDomainEvent<InventoryItemQuantityIncreasedData>(
            Guid.NewGuid(), sequenceNumber: 3, DateTime.Now, InventoryDomainEventType.QuantityIncreased, new InventoryItemQuantityIncreasedData(1));
        
        var aggregate = new InventoryItemAggregate();

        // Act
        var result = aggregate.RestoreStateFrom(new InventoryItemDomainEvent[]{ domainEventItemAdded, domainEventItemQuantityIncreased });
        
        Assert.IsTrue(result.IsFailed);
    }
}