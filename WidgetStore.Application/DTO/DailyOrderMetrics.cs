namespace WidgetStore.Application.DTO;

// One row of the read model: totals for one day
public record DailyOrderMetrics(string Day, int OrdersShipped, double TotalProcessingHours);
