using Azure;
using Azure.Data.Tables;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Interfaces;

namespace WidgetStore.Infrastructure.Tables;

 class TableOrderMetricsRepository : IOrderMetricsReadRepository, IOrderMetricsWriteRepository
{
     private readonly TableClient _table;

    public TableOrderMetricsRepository(TableServiceClient service)
    {
        _table = service.GetTableClient("OrderMetrics");
        _table.CreateIfNotExists();
    }

    // WRITE: add one shipped order to that day's totals.
    // Several Functions can run at once, so we use the ETag (optimistic concurrency):
    // "only save if nobody changed this row since I read it", otherwise read again and retry.
    public async Task AddShippedOrderAsync(DateTime shippedAt, double processingHours, CancellationToken ct)
    {
        var month = shippedAt.ToString("yyyy-MM");
        var day = shippedAt.ToString("yyyy-MM-dd");

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var existing = await _table.GetEntityIfExistsAsync<OrderMetricsEntity>(month, day, cancellationToken: ct);
            try
            {
                if (!existing.HasValue)
                {
                    await _table.AddEntityAsync(new OrderMetricsEntity
                    {
                        PartitionKey = month, RowKey = day,
                        OrdersShipped = 1, TotalProcessingHours = processingHours
                    }, ct);
                }
                else
                {
                    var row = existing.Value!;
                    row.OrdersShipped += 1;
                    row.TotalProcessingHours += processingHours;
                    await _table.UpdateEntityAsync(row, row.ETag, TableUpdateMode.Replace, ct);
                }
                return;
            }
            catch (RequestFailedException e) when (e.Status is 409 or 412)
            {
                // 409 = someone created the row first, 412 = someone changed it first → retry
            }
        }
        throw new InvalidOperationException($"Could not update metrics for {day} after 5 attempts.");
    }

    // READ: all days of one month = one partition
    public async Task<List<DailyOrderMetrics>> GetMonthAsync(string month, CancellationToken ct)
    {
        var result = new List<DailyOrderMetrics>();
        await foreach (var e in _table.QueryAsync<OrderMetricsEntity>(x => x.PartitionKey == month, cancellationToken: ct))
            result.Add(new DailyOrderMetrics(e.RowKey, e.OrdersShipped, e.TotalProcessingHours));
        return result.OrderBy(d => d.Day).ToList();
    }
}