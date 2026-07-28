using FluentValidation;
using InventoryManagement.Application.Roles.Commands;
using InventoryManagement.Application.Users.Enums;

namespace InventoryManagement.Application.Roles.Validators;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(256).WithMessage("Role name must not exceed 256 characters.");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions collection is required.")
            .Must(permissions => permissions.All(p => Enum.IsDefined(typeof(Permission), p)))
            .WithMessage("Each permission must be a valid Permission enum value.");
    }
}