using System;
using System.Threading.Tasks;
using FluentResults;
using MoverCandidateTest.Application.Events;
using MoverCandidateTest.Application.InventoryItems;
using MoverCandidateTest.Domain;
using NSubstitute;
using NUnit.Framework;

namespace WatchHandsCalculatorsTests.InventoryLogicTests;

public class InventoryItemsServiceTests
{
    private IInventoryDomainEventsRepository _repository;
    private IInventoryItemDtoValidator _validator;
    private InventoryItemsService _inventoryItemsService;
    
    [SetUp]
    public void Setup()
    {
        _repository = Substitute.For<IInventoryDomainEventsRepository>();
        _validator = Substitute.For<IInventoryItemDtoValidator>();
        _inventoryItemsService = new InventoryItemsService(_repository, _validator);
    }
    
    [Test]
    public async Task Doesnt_create_or_update_item_on_invalid_dto()
    {
        _validator.Validate(Arg.Any<InventoryItemDto>()).Returns(new [] { "validation error" });

        // Act
        var result = await _inventoryItemsService.CreateOrUpdate(new InventoryItemDto(string.Empty, "test", 1), Guid.Empty);
        
        Assert.IsTrue(result.IsFailed);
        Assert.IsTrue(result.HasError<InventoryItemsService.ValidationError>());
        await _repository.DidNotReceiveWithAnyArgs().Add(default, default);
    }

    [Test]
    public async Task Creates_new_item_when_there_are_no_events_for_sku()
    {
        var sku = "sku";
        _validator.Validate(Arg.Any<InventoryItemDto>()).Returns(Array.Empty<string>());
        _repository.GetAll(Arg.Is<string>(x => x == sku)).Returns(Array.Empty<InventoryItemDomainEvent>());
        _repository.Add(Arg.Is<string>(x => x == sku),
            Arg.Is<InventoryItemDomainEvent>(x => x.Type == InventoryDomainEventType.ItemAdded)).Returns(Result.Ok());
        
        // Act
        var result = await _inventoryItemsService.CreateOrUpdate(new InventoryItemDto(sku, "test", 1), Guid.Empty);
        
        Assert.IsTrue(result.IsSuccess);
    }
}