namespace WidgetStore.Application.Interfaces;

// Contract: "something that can put a message on a queue".
public interface IMessagePublisher
{
    Task PublishAsync<T>(string queueName, T message, CancellationToken ct);
}