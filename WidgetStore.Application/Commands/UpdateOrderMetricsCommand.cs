using MediatR;
namespace WidgetStore.Application.Commands;

public record UpdateOrderMetricsCommand(DateTime ShippedAt, double ProcessingHours) : IRequest;