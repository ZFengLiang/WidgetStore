using Microsoft.EntityFrameworkCore;
using WidgetStore.Application.Interfaces;
using WidgetStore.Domain.Products;

namespace WidgetStore.Infrastructure.Sql;

public class SqlProductRepository(WidgetStoreDbContext db) : IProductRepository, IProductWriteRepository
{
    public Task<List<Product>> GetAllAsync(string? category, CancellationToken ct)
    {
        var query = db.Products.AsNoTracking();   // read-only, so it's faster
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);
        return query.OrderBy(p => p.Name).ToListAsync(ct);
    }
    public Task<Product?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
    public Task SetImageUrlAsync(int productId, string imageUrl, CancellationToken ct) =>
        db.Products.Where(p => p.Id == productId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.ImageUrl, imageUrl), ct);
    
}