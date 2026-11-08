using Moq;
using WidgetStore.Application.Commands;
using WidgetStore.Application.Commands.Handlers;
using WidgetStore.Application.Interfaces;
using WidgetStore.Domain.Orders;

namespace WidgetStore.Tests.Handlers;

public class ReserveStockCommandHandlerTests
{
    private readonly Mock<IOrderWriteRepository> _orders = new();
    private readonly Mock<IStockService> _stock = new();

    private ReserveStockCommandHandler CreateHandler() => new(_orders.Object, _stock.Object);

    private Order GivenStoredOrder(Order order)
    {
        _orders.Setup(o => o.GetAsync(order.CustomerId, order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        return order;
    }

    [Fact]
    public async Task Not_enough_stock_rejects_the_order()
    {
        var order = GivenStoredOrder(Order.Place("cust-8", [new OrderLine(3, 5, 4.95m)], DateTime.UtcNow));
        _stock.Setup(s => s.TryReserveAllAsync(It.IsAny<IReadOnlyList<OrderLine>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var status = await CreateHandler().Handle(new ReserveStockCommand(order.Id, order.CustomerId), CancellationToken.None);

        Assert.Equal("Rejected", status);
        _orders.Verify(o => o.UpdateAsync(It.Is<Order>(x => x.Status == OrderStatus.Rejected),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Same_message_twice_does_not_reserve_stock_again()
    {
        // The order was already confirmed by the first delivery of the message
        var order = Order.Place("cust-7", [new OrderLine(2, 2, 24.95m)], DateTime.UtcNow);
        order.Confirm();
        GivenStoredOrder(order);

        var status = await CreateHandler().Handle(new ReserveStockCommand(order.Id, order.CustomerId), CancellationToken.None);

        Assert.Equal("Confirmed", status);
        _stock.Verify(s => s.TryReserveAllAsync(It.IsAny<IReadOnlyList<OrderLine>>(), It.IsAny<CancellationToken>()),
            Times.Never);   // idempotent: no second stock reservation
    }
}