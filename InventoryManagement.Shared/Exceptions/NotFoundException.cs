namespace InventoryManagement.Shared.Exceptions;

public class NotFoundException(string message) : DomainException(message)
{
}
