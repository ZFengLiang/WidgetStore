using MediatR;
using WidgetStore.Application.Interfaces;
namespace WidgetStore.Application.Products.Queries;

// QUERY = a question. Only reads, never changes anything.
public record GetProductsQuery(string? Category) : IRequest<List<ProductDto>>;

// Handler = answers the question, then MediatR finds it automatically 
public class GetProductsHandler(IProductRepository products) : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request,CancellationToken ct)
    {
        var list = await products.GetAllAsync(request.Category, ct);
        return list.Select(ProductDto.From).ToList();
    }
}
