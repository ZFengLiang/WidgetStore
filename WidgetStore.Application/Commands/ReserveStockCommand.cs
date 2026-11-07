using MediatR;
namespace WidgetStore.Application.Commands;

public record ReserveStockCommand(string OrderId, string CustomerId) : IRequest<string>;
