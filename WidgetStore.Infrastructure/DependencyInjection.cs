using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WidgetStore.Application.Interfaces;
using WidgetStore.Infrastructure.Queues;
using WidgetStore.Infrastructure.Sql;
using WidgetStore.Infrastructure.Tables;

namespace WidgetStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // SQL Server: products + stock
        services.AddDbContext<WidgetStoreDbContext>(o =>
            o.UseSqlServer(config.GetConnectionString("Sql")));
        services.AddScoped<IProductRepository, SqlProductRepository>();

        // Azure Storage (Azurite locally): tables + queues
        var storage = config.GetConnectionString("Storage");
        services.AddSingleton(new TableServiceClient(storage));
        // Base64: Azure Functions queue triggers expect Base64 messages, otherwise they never fire
        services.AddSingleton(new QueueServiceClient(storage,
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }));

        services.AddSingleton<IOrderRepository, TableOrderRepository>();
        services.AddSingleton<IMessagePublisher, StorageQueuePublisher>();
        return services;
    }
}