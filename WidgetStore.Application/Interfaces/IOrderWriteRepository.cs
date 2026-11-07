using WidgetStore.Domain.Orders;
namespace WidgetStore.Application.Interfaces;

// WRITE side: used only by commands
public interface IOrderWriteRepository
{
    Task<Order?> GetAsync(string customerId, string orderId, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task UpdateAsync(Order order, CancellationToken ct);
}