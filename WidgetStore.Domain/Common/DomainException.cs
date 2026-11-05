namespace WidgetStore.Domain.Common;

// Thrown when someone tries to break a business rule.
// The API will turn this into a "400 Bad Request" later.
public class DomainException(string message) : Exception(message);