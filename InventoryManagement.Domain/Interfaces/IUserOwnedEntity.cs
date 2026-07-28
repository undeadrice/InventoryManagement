using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Domain.Interfaces;

public interface IUserOwnedEntity : IEntity
{
    Guid UserId { get; }
}