using FluentValidation;
using InventoryManagement.Application.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Application;

public static class DIRegistrations
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DIRegistrations).Assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CheckRoleBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CheckPermissionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OwnedResourceBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(DIRegistrations).Assembly);

        return services;
    }
}

