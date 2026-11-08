namespace WidgetStore.Application.Messages;

// Integration event: put on "order-events" when an order ships, picked up by Function 3
public record OrderShipped(string OrderId, DateTime ShippedAt, double ProcessingHours);