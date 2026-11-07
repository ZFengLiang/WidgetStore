using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WidgetStore.Application.Commands;
using WidgetStore.Application.Messages;

namespace WidgetStore.Functions;

public class ProcessOrderFunction(IMediator mediator, ILogger<ProcessOrderFunction> logger)
{
    // Runs automatically for every message on the "order-placed" queue
    [Function("ProcessOrder")]
    public async Task Run(
        [QueueTrigger(QueueNames.OrderPlaced, Connection = "AzureWebJobsStorage")] string messageText,
        CancellationToken ct)
    {
        var message = JsonSerializer.Deserialize<OrderPlaced>(messageText, JsonSerializerOptions.Web)!;

        // Thin function: hand the work to the Application layer
        var status = await mediator.Send(new ReserveStockCommand(message.OrderId, message.CustomerId), ct);

        logger.LogInformation("Order {OrderId} processed: {Status}", message.OrderId, status);
    }
}