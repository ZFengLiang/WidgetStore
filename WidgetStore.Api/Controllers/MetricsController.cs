using MediatR;
using Microsoft.AspNetCore.Mvc;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Queries;
namespace WidgetStore.Api.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricsController(IMediator mediator) : ControllerBase
{
    // GET /api/metrics/order-processing?month=2026-11
    [HttpGet("order-processing")]
    public async Task<ActionResult<OrderMetricsDto>> OrderProcessing([FromQuery] string month, CancellationToken ct)
        => Ok(await mediator.Send(new GetOrderMetricsQuery(month), ct));
}