using WidgetStore.Domain.Orders;
namespace WidgetStore.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetAsync(string customerId, string orderId, CancellationToken ct);
    Task UpdateAsync(Order order, CancellationToken ct);
}