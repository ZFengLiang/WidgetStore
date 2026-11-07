using MediatR;
using WidgetStore.Application.DTO;
namespace WidgetStore.Application.Queries;

public record GetOrderQuery(string CustomerId, string OrderId) : IRequest<OrderDto?>;