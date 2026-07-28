namespace InventoryManagement.Shared.Exceptions;

public class ForbiddenException(string message = "Forbidden") : DomainException(message)
{
}