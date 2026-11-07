using MediatR;
using Microsoft.AspNetCore.Mvc;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Queries;


namespace WidgetStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IMediator mediator) : ControllerBase
{
    // Thin controller: turns HTTP into a query, nothing more.
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] string? category, CancellationToken ct)
        => Ok(await mediator.Send(new GetProductsQuery(category), ct));
}