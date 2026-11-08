using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WidgetStore.Application.Commands;
using WidgetStore.Application.Messages;

namespace WidgetStore.Functions;

public class UpdateOrderMetricsFunction(IMediator mediator, ILogger<UpdateOrderMetricsFunction> logger)
{
    // Runs for every message on "order-events": keeps the read model up to date
    [Function("UpdateOrderMetrics")]
    public async Task Run(
        [QueueTrigger(QueueNames.OrderEvents, Connection = "AzureWebJobsStorage")] string messageText,
        CancellationToken ct)
    {
        var message = JsonSerializer.Deserialize<OrderShipped>(messageText, JsonSerializerOptions.Web)!;

        await mediator.Send(new UpdateOrderMetricsCommand(message.ShippedAt, message.ProcessingHours), ct);

        logger.LogInformation("Metrics updated for order {OrderId} ({Hours} h)", message.OrderId, message.ProcessingHours);
    }
}