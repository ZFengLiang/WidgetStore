namespace WidgetStore.Application.DTO;

public record OrderLineDto(int ProductId, int Quantity, decimal UnitPrice, decimal LineTotal);