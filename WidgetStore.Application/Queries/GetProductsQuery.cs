using MediatR;
using WidgetStore.Application.DTO;
namespace WidgetStore.Application.Queries;

public record GetProductsQuery(string? Category) : IRequest<List<ProductDto>>;