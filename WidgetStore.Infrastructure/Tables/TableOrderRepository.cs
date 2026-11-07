using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using WidgetStore.Application.Interfaces;
using WidgetStore.Domain.Orders;

namespace WidgetStore.Infrastructure.Tables;

public class TableOrderRepository : IOrderRepository
{
    private readonly TableClient _table;

    public TableOrderRepository(TableServiceClient service)
    {
        _table = service.GetTableClient("Orders");
        _table.CreateIfNotExists();   // creates the table the first time
    }

    public Task AddAsync(Order order, CancellationToken ct) =>
        _table.AddEntityAsync(ToEntity(order), ct);

    public async Task<Order?> GetAsync(string customerId, string orderId, CancellationToken ct)
    {
        // Point read: PartitionKey + RowKey, the fastest and cheapest read in Table Storage
        var result = await _table.GetEntityIfExistsAsync<OrderEntity>(customerId, orderId, cancellationToken: ct);
        return result.HasValue ? ToDomain(result.Value!) : null;
    }

    public Task UpdateAsync(Order order, CancellationToken ct) =>
        _table.UpdateEntityAsync(ToEntity(order), ETag.All, TableUpdateMode.Replace, ct);

    private static OrderEntity ToEntity(Order o) => new()
    {
        PartitionKey = o.CustomerId,
        RowKey = o.Id,
        Status = o.Status.ToString(),
        OrderDate = o.OrderDate,
        ShippedAt = o.ShippedAt,
        LinesJson = JsonSerializer.Serialize(o.Lines)
    };

    private static Order ToDomain(OrderEntity e) => Order.Restore(
        e.RowKey,
        e.PartitionKey,
        JsonSerializer.Deserialize<List<OrderLine>>(e.LinesJson) ?? new(),
        Enum.Parse<OrderStatus>(e.Status),
        e.OrderDate.UtcDateTime,
        e.ShippedAt?.UtcDateTime);
}