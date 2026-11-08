using Moq;
using WidgetStore.Application.Commands;
using WidgetStore.Application.Commands.Handlers;
using WidgetStore.Application.Interfaces;
using WidgetStore.Application.Messages;
using WidgetStore.Domain.Common;
using WidgetStore.Domain.Orders;
using WidgetStore.Domain.Products;

namespace WidgetStore.Tests.Handlers;

public class PlaceOrderCommandHandlerTests
{
     // Mocks: fake versions of the contracts, so no database or queue is needed
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IOrderWriteRepository> _orders = new();
    private readonly Mock<IMessagePublisher> _publisher = new();

    private PlaceOrderCommandHandler CreateHandler() => new(_products.Object, _orders.Object, _publisher.Object);

    [Fact]
    public async Task Saves_pending_order_and_puts_message_on_queue()
    {
        // Arrange: "when asked for product 2, return this product"
        _products.Setup(p => p.GetByIdAsync(2, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Product { Id = 2, Name = "Widget Pro", Price = 24.95m, Stock = 10 });

        // Act
        var orderId = await CreateHandler().Handle(
            new PlaceOrderCommand("cust-7", [new PlaceOrderLine(2, 2)]), CancellationToken.None);

        // Assert: saved once as Pending with the right total, and exactly one queue message
        _orders.Verify(o => o.AddAsync(
            It.Is<Order>(x => x.Id == orderId && x.Status == OrderStatus.Pending && x.Total == 49.90m),
            It.IsAny<CancellationToken>()), Times.Once);

        _publisher.Verify(p => p.PublishAsync(QueueNames.OrderPlaced,
            It.Is<OrderPlaced>(m => m.OrderId == orderId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Unknown_product_is_rejected_and_nothing_is_saved()
    {
        _products.Setup(p => p.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<DomainException>(() => CreateHandler().Handle(
            new PlaceOrderCommand("cust-7", [new PlaceOrderLine(99, 1)]), CancellationToken.None));

        _orders.Verify(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _publisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<OrderPlaced>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}