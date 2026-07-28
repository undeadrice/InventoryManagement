using InventoryManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Services;

public class UserOwnershipRepository<T>(PersistenceDbContext dbContext) : IUserOwnershipRepository<T> where T : class, IUserOwnedEntity
{
    public async Task<bool> IsOwner(Guid userId, Guid resourceId)
    {
        var entity = await dbContext.Set<T>().FindAsync(resourceId);

        if (entity is null)
            return false;

        return entity.UserId == userId;
    }
}