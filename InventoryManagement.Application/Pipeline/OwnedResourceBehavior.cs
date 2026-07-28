using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Shared.Exceptions;
using MediatR;

namespace InventoryManagement.Application.Pipeline;

public class OwnedResourceBehavior<TRequest, TResponse>(
    ICurrentUserService currentUserService,
    IUserOwnershipRepository<IUserOwnedEntity> ownershipRepository)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IOwnedResourceRequest<IUserOwnedEntity>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IOwnedResourceRequest<IUserOwnedEntity>)
        {
            return await next(cancellationToken);
        }

        var ownedResourceRequest = (IOwnedResourceRequest<IUserOwnedEntity>)request;
        var userId = currentUserService.CurrentUserId ?? throw new UnauthorizedException();
        var isOwner = await ownershipRepository.IsOwner(userId, ownedResourceRequest.ResourceId);

        if (!isOwner)
        {
            throw new UnauthorizedException();
        }

        return await next(cancellationToken);
    }
}