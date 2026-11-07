using MediatR;
namespace WidgetStore.Application.Commands;

public record PlaceOrderCommand(string CustomerId, List<PlaceOrderLine> Lines) : IRequest<string>;
public record PlaceOrderLine(int ProductId, int Quantity);