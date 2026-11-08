namespace WidgetStore.Application.DTO;

public record DailyMetricsDto(string Day, int OrdersShipped, double AvgProcessingHours);
public record OrderMetricsDto(string Month, int OrdersShipped, double? AvgProcessingHours, List<DailyMetricsDto> Days);