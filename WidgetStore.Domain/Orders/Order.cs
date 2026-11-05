using WidgetStore.Domain.Common;

namespace WidgetStore.Domain.Orders;

public class Order
{
    public const int MaxQuantityPerLine = 10;

    // "private set": only the Order itself can change these, so nobody can skip the rules
    public string Id { get; private set; } = "";
    public string CustomerId { get; private set; } = "";
    public List<OrderLine> Lines { get; private set; } = new();
    public OrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ShippedAt { get; private set; }

    // Calculated, never stored separately, so it can never be out of date
    public decimal Total => Lines.Sum(l => l.LineTotal);

    // The "orderprocessed" metric from the assignment: time between ordering and shipping
    public double? ProcessingHours =>
        ShippedAt is null ? null : Math.Round((ShippedAt.Value - OrderDate).TotalHours, 1);

    // Private constructor: the ONLY way to make a new order is Place(), which checks the rules
    private Order() { }

    public static Order Place(string customerId, List<OrderLine> lines, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new DomainException("An order needs a customer.");
        if (lines.Count == 0)
            throw new DomainException("An order needs at least one line.");
        if (lines.Any(l => l.Quantity < 1 || l.Quantity > MaxQuantityPerLine))
            throw new DomainException($"Quantity must be between 1 and {MaxQuantityPerLine}.");

        return new Order
        {
            Id = Guid.NewGuid().ToString("N"),
            CustomerId = customerId,
            Lines = lines,
            Status = OrderStatus.Pending,
            OrderDate = now
        };
    }

    // Status changes: each one checks that the order is in the right state first
    public void Confirm()
    {
        EnsureStatus(OrderStatus.Pending);
        Status = OrderStatus.Confirmed;
    }

    public void Reject()
    {
        EnsureStatus(OrderStatus.Pending);
        Status = OrderStatus.Rejected;
    }

    public void Ship(DateTime now)
    {
        EnsureStatus(OrderStatus.Confirmed);   // you can't ship a rejected or pending order
        Status = OrderStatus.Shipped;
        ShippedAt = now;
    }

    private void EnsureStatus(OrderStatus expected)
    {
        if (Status != expected)
            throw new DomainException($"Order is {Status}, but must be {expected} for this action.");
    }

    // Used only when loading an existing order from the database (no rules, it was already valid)
    public static Order Restore(string id, string customerId, List<OrderLine> lines,
        OrderStatus status, DateTime orderDate, DateTime? shippedAt)
        => new()
        {
            Id = id, CustomerId = customerId, Lines = lines,
            Status = status, OrderDate = orderDate, ShippedAt = shippedAt
        };
}