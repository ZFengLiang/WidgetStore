using WidgetStore.Application.DTO;
namespace WidgetStore.Application.Interfaces;

public interface IOrderMetricsReadRepository
{
    Task<List<DailyOrderMetrics>> GetMonthAsync(string month, CancellationToken ct);
}