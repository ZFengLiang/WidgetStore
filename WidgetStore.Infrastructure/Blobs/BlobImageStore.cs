using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using WidgetStore.Application.Interfaces;
namespace WidgetStore.Infrastructure.Blobs;

public class BlobImageStore : IImageStore
{
    private readonly BlobContainerClient _container;

    public BlobImageStore(BlobServiceClient service)
    {
        _container = service.GetBlobContainerClient("products");
        // Public read for images only (not for listing the container), so browsers can load them directly
        _container.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadAsync(string blobName, Stream content, string contentType, CancellationToken ct)
    {
        var blob = _container.GetBlobClient(blobName);
        await blob.UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }   // so the browser shows it as an image
        }, ct);
        return blob.Uri.ToString();
    }
}