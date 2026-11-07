using MediatR;
using WidgetStore.Application.Interfaces;
using WidgetStore.Application.Messages;
using WidgetStore.Domain.Common;
using WidgetStore.Domain.Orders;

namespace WidgetStore.Application.Commands.Handlers;

public class PlaceOrderCommandHandler(IProductRepository products, IOrderWriteRepository orders, IMessagePublisher publisher)
    : IRequestHandler<PlaceOrderCommand, string>
{
    public async Task<string> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        // Prices come from OUR database, never from the customer
        var lines = new List<OrderLine>();
        foreach (var line in cmd.Lines)
        {
            var product = await products.GetByIdAsync(line.ProductId, ct)
                          ?? throw new DomainException($"Product {line.ProductId} does not exist.");
            lines.Add(new OrderLine(product.Id, line.Quantity, product.Price));
        }

        var order = Order.Place(cmd.CustomerId, lines, DateTime.UtcNow);   // rules checked in Domain

        await orders.AddAsync(order, ct);                                   // 1. save as Pending
        await publisher.PublishAsync(QueueNames.OrderPlaced,                // 2. ticket for Function 1
            new OrderPlaced(order.Id, order.CustomerId), ct);

        return order.Id;                                                    // 3. answer fast
    }
}