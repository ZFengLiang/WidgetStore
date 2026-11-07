using WidgetStore.Domain.Products;
namespace WidgetStore.Application.Products;

// What the API sends back. We never expose the Domain class directly.
public record ProductDto(
    int Id,
    string Name,
    string Description,
    string Category,
    decimal Price,
    int Stock,
    string? ImageUrl)
{
    public static ProductDto From(Product p) =>
        new(p.Id, p.Name, p.Description, p.Category, p.Price, p.Stock, p.ImageUrl); 
}
    