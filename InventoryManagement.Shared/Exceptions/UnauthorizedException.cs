namespace InventoryManagement.Shared.Exceptions;

public class UnauthorizedException(string message = "Unauthorized") : DomainException(message)
{
}