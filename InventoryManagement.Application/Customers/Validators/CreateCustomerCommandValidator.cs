using FluentValidation;
using InventoryManagement.Application.Customers.Commands;
using InventoryManagement.Domain.Customers;

namespace InventoryManagement.Application.Customers.Validators;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Location)
            .IsInEnum().WithMessage($"Location must be one of: {string.Join(", ", Enum.GetNames<CustomerLocation>())}.");
    }
}
