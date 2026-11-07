using WidgetStore.Domain.Orders;
namespace WidgetStore.Application.Interfaces;

// READ side: used only by queries
public interface IOrderReadRepository
{
    Task<Order?> GetAsync(string customerId, string orderId, CancellationToken ct);
}