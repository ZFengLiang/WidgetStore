using WidgetStore.Domain.Orders;
namespace WidgetStore.Application.Interfaces;

public interface IStockService
{
    // true = all lines reserved; false = not enough stock, nothing changed
    Task<bool> TryReserveAllAsync(IReadOnlyList<OrderLine> lines, CancellationToken ct);
}