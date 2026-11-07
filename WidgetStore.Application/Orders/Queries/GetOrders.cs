using MediatR;
using WidgetStore.Application.Interfaces;

namespace WidgetStore.Application.Orders.Queries;

public record GetOrderQuery(string CustomerId, string OrderId) : IRequest<OrderDto?>;

public class GetOrderHandler(IOrderRepository orders) : IRequestHandler<GetOrderQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderQuery q, CancellationToken ct)
    {
        var order = await orders.GetAsync(q.CustomerId, q.OrderId, ct);
        return order is null ? null : OrderDto.From(order);
    }
}