using MediatR;
using WidgetStore.Application.Interfaces;
using WidgetStore.Domain.Common;
using WidgetStore.Domain.Orders;

namespace WidgetStore.Application.Orders.Commands;

// COMMAND = a request to CHANGE something. Returns the new order id.
public record PlaceOrderCommand(string CustomerId, List<PlaceOrderLine> Lines) : IRequest<string>;
public record PlaceOrderLine(int ProductId, int Quantity);

public class PlaceOrderHandler(IProductRepository products, IOrderRepository orders, IMessagePublisher publisher)
    : IRequestHandler<PlaceOrderCommand, string>
{
    public async Task<string> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        // Look up the current price of each product (price comes from OUR database, never from the customer)
        var lines = new List<OrderLine>();
        foreach (var line in cmd.Lines)
        {
            var product = await products.GetByIdAsync(line.ProductId, ct)
                          ?? throw new DomainException($"Product {line.ProductId} does not exist.");
            lines.Add(new OrderLine(product.Id, line.Quantity, product.Price));
        }

        // Business rules are checked inside Order.Place (Domain)
        var order = Order.Place(cmd.CustomerId, lines, DateTime.UtcNow);

        await orders.AddAsync(order, ct);                                   // 1. save as Pending
        await publisher.PublishAsync(QueueNames.OrderPlaced,                // 2. ticket for Function 1
            new OrderPlaced(order.Id, order.CustomerId), ct);

        return order.Id;                                                    // 3. answer fast
    }
}