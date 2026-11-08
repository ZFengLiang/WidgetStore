using MediatR;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Interfaces;
using WidgetStore.Application.Messages;
namespace WidgetStore.Application.Commands.Handlers;

public class ShipOrderCommandHandler(IOrderWriteRepository orders, IMessagePublisher publisher)
    : IRequestHandler<ShipOrderCommand, OrderDto?>
{
    public async Task<OrderDto?> Handle(ShipOrderCommand cmd, CancellationToken ct)
    {
        var order = await orders.GetAsync(cmd.CustomerId, cmd.OrderId, ct);
        if (order is null)
            return null;

        order.Ship(DateTime.UtcNow);            // Domain rule: only Confirmed orders can ship
        await orders.UpdateAsync(order, ct);

        // Tell the read side: Function 3 will update the metrics
        await publisher.PublishAsync(QueueNames.OrderEvents,
            new OrderShipped(order.Id, order.ShippedAt!.Value, order.ProcessingHours!.Value), ct);

        return OrderDto.From(order);
    }
}