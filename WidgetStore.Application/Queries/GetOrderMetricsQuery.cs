using MediatR;
using WidgetStore.Application.DTO;
namespace WidgetStore.Application.Queries;

public record GetOrderMetricsQuery(string Month) : IRequest<OrderMetricsDto>;
