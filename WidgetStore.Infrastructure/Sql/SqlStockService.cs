using Microsoft.EntityFrameworkCore;
using WidgetStore.Application.Interfaces;
using WidgetStore.Domain.Orders;
namespace WidgetStore.Infrastructure.Sql;

public class SqlStockService(WidgetStoreDbContext db) : IStockService
{
    public async Task<bool> TryReserveAllAsync(IReadOnlyList<OrderLine> lines, CancellationToken ct)
    {
        // One transaction: every line succeeds, or nothing changes
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        foreach (var line in lines)
        {
            // UPDATE Products SET Stock = Stock - qty WHERE Id = id AND Stock >= qty
            var changed = await db.Products
                .Where(p => p.Id == line.ProductId && p.Stock >= line.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - line.Quantity), ct);

            if (changed == 0)   // not enough stock for this line
            {
                await tx.RollbackAsync(ct);
                return false;
            }
        }

        await tx.CommitAsync(ct);
        return true;
    }
}