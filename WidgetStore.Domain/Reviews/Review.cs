using WidgetStore.Domain.Common;

namespace WidgetStore.Domain.Reviews;

// Anonymous by design: there is NO CustomerId property
// so make it impossible to store who wrote it 
public class Review
{
    public const int MaxTextLength = 1000;

    public string Id { get; private set; } = "";
    public int ProductId { get; private set; }
    public int Rating { get; private set; }
    public string Text { get; private set; } = "";
    public DateTime CreatedAt { get; private set; }

    private Review() { }

    public static Review Create(int productId, int rating, string text, DateTime now)
    {
        if (rating is < 1 or > 5)
            throw new DomainException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("A review needs text.");
        if (text.Length > MaxTextLength)
            throw new DomainException($"A review can be at most {MaxTextLength} characters.");

        return new Review
        {
            Id = Guid.NewGuid().ToString("N"),
            ProductId = productId,
            Rating = rating,
            Text = text.Trim(),
            CreatedAt = now
        };
    }

    public static Review Restore(string id, int productId, int rating, string text, DateTime createdAt)
        => new() { Id = id, ProductId = productId, Rating = rating, Text = text, CreatedAt = createdAt };
}