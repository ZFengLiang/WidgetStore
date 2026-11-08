using MediatR;
using Microsoft.AspNetCore.Mvc;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Queries;
using WidgetStore.Application.Commands;


namespace WidgetStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IMediator mediator) : ControllerBase
{
    // Thin controller: turns HTTP into a query, nothing more.
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] string? category, CancellationToken ct)
        => Ok(await mediator.Send(new GetProductsQuery(category), ct));
    
    
    // POST /api/products/{id}/image  (multipart form with a "file" field)
    [HttpPost("{id:int}/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file, CancellationToken ct)
    {
        if (!file.ContentType.StartsWith("image/"))
            return BadRequest(new { error = "Only image files are allowed." });
        if (file.Length > 2 * 1024 * 1024)
            return BadRequest(new { error = "An image can be at most 2 MB." });

        await using var stream = file.OpenReadStream();
        var url = await mediator.Send(new UploadProductImageCommand(id, stream, file.FileName, file.ContentType), ct);
        return url is null ? NotFound() : Ok(new { productId = id, imageUrl = url });
    }
    
}