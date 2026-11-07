using MediatR;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Interfaces;namespace WidgetStore.Application.Queries.Handlers;

public class GetOrderQueryHandler(IOrderReadRepository orders)
    : IRequestHandler<GetOrderQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderQuery q, CancellationToken ct)
    {
        var order = await orders.GetAsync(q.CustomerId, q.OrderId, ct);
        return order is null ? null : OrderDto.From(order);
    }
}