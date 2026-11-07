using MediatR;
using WidgetStore.Application.DTO;
using WidgetStore.Application.Interfaces;
namespace WidgetStore.Application.Queries.Handlers;

public class GetProductsQueryHandler(IProductRepository products)
    : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var list = await products.GetAllAsync(request.Category, ct);
        return list.Select(ProductDto.From).ToList();
    }
}