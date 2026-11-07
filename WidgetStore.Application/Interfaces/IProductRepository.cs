using WidgetStore.Domain.Products;
namespace WidgetStore.Application.Interfaces;

// Contract: "something that can fetch products". Application doesn't care HOW.
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(string? category, CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
}