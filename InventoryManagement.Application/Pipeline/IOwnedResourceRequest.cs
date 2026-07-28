using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Application.Pipeline;

public interface IOwnedResourceRequest<TEntity> where TEntity : IUserOwnedEntity
{
    Guid ResourceId { get; init; }
}