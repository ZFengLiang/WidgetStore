namespace WidgetStore.Domain.Orders;

// One line on an order: "2 × Widget Pro at €24.95".
// A record is a small class that can't change after creation; that's what an order line should be.
public record OrderLine(int ProductId, int Quantity, decimal UnitPrice)
{
    public decimal LineTotal => Quantity * UnitPrice;
}
