using Moq;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Interfaces;
using WidgetStore.Application.Queries;
using WidgetStore.Application.Queries.Handlers;

namespace WidgetStore.Tests.Handlers;

public class GetOrderMetricsQueryHandlerTests
{
    private readonly Mock<IOrderMetricsReadRepository> _metrics = new();

    [Fact]
    public async Task Average_is_total_hours_divided_by_orders_shipped()
    {
        _metrics.Setup(m => m.GetMonthAsync("2026-11", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyOrderMetrics>
            {
                new("2026-11-07", 1, 6.0),
                new("2026-11-08", 3, 30.0)
            });

        var result = await new GetOrderMetricsQueryHandler(_metrics.Object)
            .Handle(new GetOrderMetricsQuery("2026-11"), CancellationToken.None);

        Assert.Equal(4, result.OrdersShipped);
        Assert.Equal(9.0, result.AvgProcessingHours);   // (6 + 30) / 4
        Assert.Equal(2, result.Days.Count);
    }

    [Fact]
    public async Task Month_without_shipped_orders_has_no_average()
    {
        _metrics.Setup(m => m.GetMonthAsync("2026-12", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DailyOrderMetrics>());

        var result = await new GetOrderMetricsQueryHandler(_metrics.Object)
            .Handle(new GetOrderMetricsQuery("2026-12"), CancellationToken.None);

        Assert.Equal(0, result.OrdersShipped);
        Assert.Null(result.AvgProcessingHours);
    }
}