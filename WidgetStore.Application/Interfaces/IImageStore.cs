namespace WidgetStore.Application.Interfaces;

public interface IImageStore
{
    // Saves the file and returns its public URL
    Task<string> UploadAsync(string blobName, Stream content, string contentType, CancellationToken ct);
}