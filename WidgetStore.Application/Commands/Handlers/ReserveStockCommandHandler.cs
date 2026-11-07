using MediatR;
using WidgetStore.Application.Interfaces;
using WidgetStore.Domain.Orders;
namespace WidgetStore.Application.Commands.Handlers;

public class ReserveStockCommandHandler(IOrderWriteRepository orders, IStockService stock)
    : IRequestHandler<ReserveStockCommand, string>
{
    public async Task<string> Handle(ReserveStockCommand cmd, CancellationToken ct)
    {
        var order = await orders.GetAsync(cmd.CustomerId, cmd.OrderId, ct);
        if (order is null)
            return "NotFound";

        // Idempotent: if the same message arrives twice, the second time does nothing
        if (order.Status != OrderStatus.Pending)
            return order.Status.ToString();

        var reserved = await stock.TryReserveAllAsync(order.Lines, ct);
        if (reserved) order.Confirm();
        else order.Reject();

        await orders.UpdateAsync(order, ct);
        return order.Status.ToString();
    }
}