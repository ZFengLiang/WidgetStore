using WidgetStore.Application.Products.Queries;
using WidgetStore.Infrastructure;
using WidgetStore.Infrastructure.Sql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o => o.Filters.Add<WidgetStore.Api.Filters.DomainExceptionFilter>());
builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductsQuery).Assembly));
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Create the database and seed products on startup (fine for a proof of concept)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WidgetStoreDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();