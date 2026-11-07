using System.Text.Json;
using Azure.Storage.Queues;
using WidgetStore.Application.Interfaces;
using WidgetStore.Application.Orders;
namespace WidgetStore.Infrastructure.Queues;

public class StorageQueuePublisher : IMessagePublisher
{
    private readonly QueueServiceClient _service;

    public StorageQueuePublisher(QueueServiceClient service)
    {
        _service = service;
        // Create our queues once at startup
        _service.GetQueueClient(QueueNames.OrderPlaced).CreateIfNotExists();
        _service.GetQueueClient(QueueNames.OrderEvents).CreateIfNotExists();
    }

    public Task PublishAsync<T>(string queueName, T message, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(message, JsonSerializerOptions.Web);   // camelCase JSON
        return _service.GetQueueClient(queueName).SendMessageAsync(json, ct);
    }
}