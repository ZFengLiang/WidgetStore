namespace WidgetStore.Application.Interfaces;

public interface IOrderMetricsWriteRepository
{
    Task AddShippedOrderAsync(DateTime shippedAt, double processingHours, CancellationToken ct);
}