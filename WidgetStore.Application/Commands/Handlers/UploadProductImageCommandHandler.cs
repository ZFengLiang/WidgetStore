using MediatR;
using WidgetStore.Application.Interfaces;
namespace WidgetStore.Application.Commands.Handlers;

public class UploadProductImageCommandHandler(
    IProductRepository products, IProductWriteRepository productWriter, IImageStore images)
    : IRequestHandler<UploadProductImageCommand, string?>
{
    public async Task<string?> Handle(UploadProductImageCommand cmd, CancellationToken ct)
    {
        if (await products.GetByIdAsync(cmd.ProductId, ct) is null)
            return null;

        // Unique name per upload, grouped per product: "2/3f9c...png"
        var blobName = $"{cmd.ProductId}/{Guid.NewGuid():N}{Path.GetExtension(cmd.FileName)}";

        var url = await images.UploadAsync(blobName, cmd.Content, cmd.ContentType, ct);   // file → Blob
        await productWriter.SetImageUrlAsync(cmd.ProductId, url, ct);                     // link → SQL
        return url;
    }
}