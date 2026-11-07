using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WidgetStore.Application.Interfaces;
using WidgetStore.Infrastructure.Sql;

namespace WidgetStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<WidgetStoreDbContext>(o =>
            o.UseSqlServer(config.GetConnectionString("Sql")));

        services.AddScoped<IProductRepository, SqlProductRepository>();
        return services;
    }
}