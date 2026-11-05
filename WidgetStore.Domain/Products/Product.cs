namespace WidgetStore.Domain.Products;


// A product lives in SQL Server. It's simple data
// Entity Framework fills these properties when reading from the database
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }  // link to the image in Blob Storage
}