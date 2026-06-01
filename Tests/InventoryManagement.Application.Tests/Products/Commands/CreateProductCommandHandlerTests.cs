using FluentAssertions;
using InventoryManagement.Application.Products.Commands;
using InventoryManagement.Domain.Products.Entities;
using InventoryManagement.Domain.Products.Services;
using NSubstitute;
using Xunit;

namespace InventoryManagement.Application.Tests.Products.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new CreateProductCommandHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddProductToRepositoryAndReturnProductId()
    {
        // Arrange
        var command = new CreateProductCommand("Laptop", "High-end laptop", 999.99m, 10);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        await _productRepository.Received(1).Add(Arg.Any<Product>());
    }

    [Fact]
    public async Task Handle_ShouldPassProductDetailsToRepository()
    {
        // Arrange
        var name = "Monitor";
        var description = "4K Monitor";
        var price = 399.99m;
        var stock = 5;
        var command = new CreateProductCommand(name, description, price, stock);

        Product? capturedProduct = null;

        _productRepository.Add(Arg.Any<Product>())
            .Returns(x =>
            {
                capturedProduct = x.Arg<Product>();
                return Task.CompletedTask;
            });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct.Name.Should().Be(name);
        capturedProduct.Description.Should().Be(description);
        capturedProduct.Price.Should().Be(price);
        capturedProduct.Stock.Should().Be(stock);
    }
}
