using MediatR;
using WidgetStore.Application.DTO;

namespace WidgetStore.Application.Commands;

// Returns the updated order, or null when the order doesn't exist
public record ShipOrderCommand(string CustomerId, string OrderId) : IRequest<OrderDto?>;
