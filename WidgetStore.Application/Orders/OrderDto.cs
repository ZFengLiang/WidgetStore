using WidgetStore.Domain.Orders;
namespace WidgetStore.Application.Orders;

public record OrderLineDto(int ProductId, int Quantity, decimal UnitPrice, decimal LineTotal);

public record OrderDto(string Id, string CustomerId, string Status, decimal Total,
    DateTime OrderDate, DateTime? ShippedAt, double? ProcessingHours, List<OrderLineDto> Lines)
{
    public static OrderDto From(Order o) => new(
        o.Id, o.CustomerId, o.Status.ToString(), o.Total, o.OrderDate, o.ShippedAt, o.ProcessingHours,
        o.Lines.Select(l => new OrderLineDto(l.ProductId, l.Quantity, l.UnitPrice, l.LineTotal)).ToList());
}