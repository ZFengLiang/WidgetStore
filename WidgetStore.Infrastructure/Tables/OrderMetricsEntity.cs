using Azure;
using Azure.Data.Tables; 

namespace WidgetStore.Infrastructure.Tables;

public class OrderMetricsEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "";
    public string RowKey { get; set; } = "";
    public int OrdersShipped { get; set; }
    public double TotalProcessingHours { get; set; }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}