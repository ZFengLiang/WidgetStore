using MediatR;
using Microsoft.AspNetCore.Mvc;
using WidgetStore.Application.Commands;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Queries;

namespace WidgetStore.Api.Controllers;

[ApiController]
[Route("api")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    // POST /api/orders → 202 Accepted: received, processed in the background
    [HttpPost("orders")]
    public async Task<IActionResult> Place(PlaceOrderCommand command, CancellationToken ct)
    {
        var orderId = await mediator.Send(command, ct);
        return Accepted(new { orderId, customerId = command.CustomerId, status = "Pending" });
    }

    // GET /api/customers/{customerId}/orders/{orderId} → both keys, so it's a point read
    [HttpGet("customers/{customerId}/orders/{orderId}")]
    public async Task<ActionResult<OrderDto>> Get(string customerId, string orderId, CancellationToken ct)
    {
        var order = await mediator.Send(new GetOrderQuery(customerId, orderId), ct);
        return order is null ? NotFound() : Ok(order);
    }
}