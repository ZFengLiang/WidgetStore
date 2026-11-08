using WidgetStore.Domain.Common;
using WidgetStore.Domain.Orders;

namespace WidgetStore.Tests.Domain;

public class OrderTests
{
    // Fixed time, so results are predictable (that's why Order takes "now" as a parameter)
    private static readonly DateTime Now = new(2026, 11, 8, 10, 0, 0, DateTimeKind.Utc);

    private static List<OrderLine> OneLine(int quantity = 2) => [new OrderLine(2, quantity, 24.95m)];

    [Fact]
    public void Place_without_lines_is_rejected() =>
        Assert.Throws<DomainException>(() => Order.Place("cust-1", [], Now));

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Place_with_invalid_quantity_is_rejected(int quantity) =>
        Assert.Throws<DomainException>(() => Order.Place("cust-1", OneLine(quantity), Now));

    [Fact]
    public void New_order_is_pending_and_total_is_sum_of_lines()
    {
        var order = Order.Place("cust-1", [new OrderLine(2, 2, 24.95m), new OrderLine(4, 1, 12.50m)], Now);

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(62.40m, order.Total);
    }

    [Fact]
    public void Pending_order_cannot_be_shipped()
    {
        var order = Order.Place("cust-1", OneLine(), Now);

        Assert.Throws<DomainException>(() => order.Ship(Now.AddHours(1)));
    }

    [Fact]
    public void ProcessingHours_is_time_between_ordering_and_shipping()
    {
        var order = Order.Place("cust-1", OneLine(), Now);
        order.Confirm();
        order.Ship(Now.AddHours(17.5));

        Assert.Equal(17.5, order.ProcessingHours);
    }
}