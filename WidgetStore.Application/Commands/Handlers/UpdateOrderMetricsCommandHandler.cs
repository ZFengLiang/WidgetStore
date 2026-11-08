using MediatR;
using WidgetStore.Application.Interfaces;
namespace WidgetStore.Application.Commands.Handlers;

public class UpdateOrderMetricsCommandHandler(IOrderMetricsWriteRepository metrics)
    : IRequestHandler<UpdateOrderMetricsCommand>
{
    public Task Handle(UpdateOrderMetricsCommand cmd, CancellationToken ct) =>
        metrics.AddShippedOrderAsync(cmd.ShippedAt, cmd.ProcessingHours, ct);
}