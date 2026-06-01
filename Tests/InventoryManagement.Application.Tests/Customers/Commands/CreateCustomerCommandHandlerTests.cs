using FluentAssertions;
using InventoryManagement.Application.Customers.Commands;
using InventoryManagement.Domain.Customers;
using InventoryManagement.Domain.Customers.Services;
using NSubstitute;
using Xunit;

namespace InventoryManagement.Application.Tests.Customers.Commands;

public class CreateCustomerCommandHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _handler = new CreateCustomerCommandHandler(_customerRepository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddCustomerToRepositoryAndReturnCustomerId()
    {
        // Arrange
        var location = CustomerLocation.US;
        var command = new CreateCustomerCommand(location);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        await _customerRepository.Received(1).Add(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }
}
