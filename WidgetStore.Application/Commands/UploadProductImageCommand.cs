using MediatR;
namespace WidgetStore.Application.Commands;

public record UploadProductImageCommand(int ProductId, Stream Content, string FileName, string ContentType)
    : IRequest<string?>;
