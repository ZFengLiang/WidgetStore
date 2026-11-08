namespace WidgetStore.Application.Interfaces;

public interface IProductWriteRepository
{
    Task SetImageUrlAsync(int productId, string imageUrl, CancellationToken ct);
}