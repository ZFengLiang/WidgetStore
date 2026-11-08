using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WidgetStore.Application.Interfaces;
using WidgetStore.Infrastructure.Queues;
using WidgetStore.Infrastructure.Sql;
using WidgetStore.Infrastructure.Tables;
using WidgetStore.Infrastructure.Blobs;

namespace WidgetStore.Infrastructure;

// One place that says "for this contract, use this class". Api and Functions both call it.
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        //SQL Server: products, stock, image links 
        services.AddDbContext<WidgetStoreDbContext>(o =>
            o.UseSqlServer(config.GetConnectionString("Sql")));
        services.AddScoped<IProductRepository, SqlProductRepository>();
        services.AddScoped<IProductWriteRepository, SqlProductRepository>();
        services.AddScoped<IStockService, SqlStockService>();

        //Azure Storage (Azurite locally) 
        var storage = config.GetConnectionString("Storage");

        // Table Storage: orders (write model) and order metrics (read model).
        // One instance per repository, exposed as a read contract and a write contract (CQRS).
        services.AddSingleton(new TableServiceClient(storage));
        services.AddSingleton<TableOrderRepository>();
        services.AddSingleton<IOrderReadRepository>(sp => sp.GetRequiredService<TableOrderRepository>());
        services.AddSingleton<IOrderWriteRepository>(sp => sp.GetRequiredService<TableOrderRepository>());
        services.AddSingleton<TableOrderMetricsRepository>();
        services.AddSingleton<IOrderMetricsReadRepository>(sp => sp.GetRequiredService<TableOrderMetricsRepository>());
        services.AddSingleton<IOrderMetricsWriteRepository>(sp => sp.GetRequiredService<TableOrderMetricsRepository>());

        // Queue Storage: order-placed and order-events.
        // Base64: Azure Functions queue triggers expect Base64 messages, otherwise they never fire.
        services.AddSingleton(new QueueServiceClient(storage,
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }));
        services.AddSingleton<IMessagePublisher, StorageQueuePublisher>();

        // Blob Storage: product image files (SQL only stores the link)
        services.AddSingleton(new BlobServiceClient(storage));
        services.AddSingleton<IImageStore, BlobImageStore>();

        return services;
    }
}