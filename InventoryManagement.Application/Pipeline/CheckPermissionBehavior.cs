using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Shared.Exceptions;
using MediatR;
using System.Reflection;

namespace InventoryManagement.Application.Pipeline;

public class CheckPermissionBehavior<TRequest, TResponse>(ICurrentUserService currentUserService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<CheckPermissionAttribute>();

        if (attribute is null)
        {
            return await next();
        }

        if (await currentUserService.IsSuperAdmin())
        {
            return await next();
        }

        if (!await currentUserService.HasPermissions(attribute.Permissions))
        {
            throw new ForbiddenException();
        }

        return await next();
    }
}