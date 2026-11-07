using Azure;
using Azure.Data.Tables;

namespace WidgetStore.Infrastructure.Tables;

// How an order looks as a row in Table Storage.
// PartitionKey = CustomerId, RowKey = OrderId (see the README reasoning below)
public class OrderEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "";   // customerId
    public string RowKey { get; set; } = "";         // orderId
    public string Status { get; set; } = "";
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }

    // Table Storage can't store a list, so the order lines are stored as one JSON text column
    public string LinesJson { get; set; } = "[]";

    // Required by Table Storage
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}