namespace WidgetStore.Application.Orders;


// Queue names in one place, so the API and the Functions always match
public static class QueueNames
{
    public const string OrderPlaced = "order-placed";
    public const string OrderEvents = "order-events";
}

// The message on the queue: just enough to find the order again
public record OrderPlaced(string OrderId, string CustomerId);