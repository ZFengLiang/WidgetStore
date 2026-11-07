using Microsoft.EntityFrameworkCore;
using WidgetStore.Domain.Products;
namespace WidgetStore.Infrastructure.Sql;

// EF Core's "connection" to the database: one DbSet per table.
public class WidgetStoreDbContext(DbContextOptions<WidgetStoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(200).IsRequired();
            e.Property(p => p.Category).HasMaxLength(100).IsRequired();
            e.Property(p => p.Price).HasPrecision(10, 2);

            // Index: "products in a category" is the most common query, so make it fast
            e.HasIndex(p => p.Category);

            // The database itself refuses negative stock: a safety net under our code
            e.ToTable(t => t.HasCheckConstraint("CK_Products_Stock", "[Stock] >= 0"));

            e.HasData(
                new Product { Id = 1, Name = "Widget Basic", Description = "The classic widget.", Category = "widgets", Price = 9.95m, Stock = 500 },
                new Product { Id = 2, Name = "Widget Pro", Description = "Stronger, faster widget.", Category = "widgets", Price = 24.95m, Stock = 140 },
                new Product { Id = 3, Name = "Widget Mini", Description = "Fits in your pocket.", Category = "widgets", Price = 4.95m, Stock = 3 },
                new Product { Id = 4, Name = "Widget Stand", Description = "Holds any widget.", Category = "accessories", Price = 12.50m, Stock = 80 }
            );
        });
    }
}