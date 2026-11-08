using MediatR;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Interfaces;

namespace WidgetStore.Application.Queries.Handlers;

public class GetOrderMetricsQueryHandler(IOrderMetricsReadRepository metrics)
    : IRequestHandler<GetOrderMetricsQuery, OrderMetricsDto>
{
    public async Task<OrderMetricsDto> Handle(GetOrderMetricsQuery q, CancellationToken ct)
    {
        var days = await metrics.GetMonthAsync(q.Month, ct);   // one partition = one fast read

        var shipped = days.Sum(d => d.OrdersShipped);
        var hours = days.Sum(d => d.TotalProcessingHours);

        return new OrderMetricsDto(
            q.Month,
            shipped,
            shipped == 0 ? null : Math.Round(hours / shipped, 2),
            days.Select(d => new DailyMetricsDto(d.Day, d.OrdersShipped,
                Math.Round(d.TotalProcessingHours / d.OrdersShipped, 2))).ToList());
    }
}