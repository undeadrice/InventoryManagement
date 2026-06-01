using FluentAssertions;
using InventoryManagement.Domain.Products.Entities;
using InventoryManagement.Domain.Products.Exceptions;
using Xunit;

namespace InventoryManagement.Domain.Tests.Products.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var name = "Laptop";
        var description = "High-performance laptop";
        var price = 999.99m;
        var stock = 10;

        // Act
        var product = Product.Create(name, description, price, stock);

        // Assert
        product.Should().NotBeNull();
        product.Id.Should().NotBe(Guid.Empty);
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Stock.Should().Be(stock);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var product1 = Product.Create("Product1", "Description1", 10m, 5);
        var product2 = Product.Create("Product2", "Description2", 20m, 10);

        // Assert
        product1.Id.Should().NotBe(product2.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowProductNameRequiredException(string? invalidName)
    {
        // Act
        var act = () => Product.Create(invalidName!, "Valid description", 10m, 5);

        // Assert
        act.Should().Throw<ProductNameRequiredException>();
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowProductNameTooLongException()
    {
        // Arrange
        var nameTooLong = new string('a', 51);

        // Act
        var act = () => Product.Create(nameTooLong, "Valid description", 10m, 5);

        // Assert
        act.Should().Throw<ProductNameTooLongException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidDescription_ShouldThrowProductDescriptionRequiredException(string? invalidDescription)
    {
        // Act
        var act = () => Product.Create("Valid name", invalidDescription!, 10m, 5);

        // Assert
        act.Should().Throw<ProductDescriptionRequiredException>();
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldThrowProductDescriptionTooLongException()
    {
        // Arrange
        var descriptionTooLong = new string('a', 51);

        // Act
        var act = () => Product.Create("Valid name", descriptionTooLong, 10m, 5);

        // Assert
        act.Should().Throw<ProductDescriptionTooLongException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-0.01)]
    public void Create_WithInvalidPrice_ShouldThrowProductPriceInvalidException(decimal invalidPrice)
    {
        // Act
        var act = () => Product.Create("Valid name", "Valid description", invalidPrice, 5);

        // Assert
        act.Should().Throw<ProductPriceInvalidException>();
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowProductStockInvalidException()
    {
        // Act
        var act = () => Product.Create("Valid name", "Valid description", 10m, -1);

        // Assert
        act.Should().Throw<ProductStockInvalidException>();
    }

    [Fact]
    public void Create_WithZeroStock_ShouldCreateProductSuccessfully()
    {
        // Act
        var product = Product.Create("Valid name", "Valid description", 10m, 0);

        // Assert
        product.Should().NotBeNull();
        product.Stock.Should().Be(0);
    }

    [Fact]
    public void DecreaseStock_WithValidQuantity_ShouldDecreaseStockCorrectly()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", 999.99m, 10);

        // Act
        product.DecreaseStock(3);

        // Assert
        product.Stock.Should().Be(7);
    }

    [Fact]
    public void DecreaseStock_MultipleDecreases_ShouldDecreaseStockCorrectly()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", 999.99m, 10);

        // Act
        product.DecreaseStock(3);
        product.DecreaseStock(2);
        product.DecreaseStock(1);

        // Assert
        product.Stock.Should().Be(4);
    }

    [Fact]
    public void DecreaseStock_ToZero_ShouldBePossible()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", 999.99m, 5);

        // Act
        product.DecreaseStock(5);

        // Assert
        product.Stock.Should().Be(0);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void DecreaseStock_WithNegativeQuantity_ShouldThrowProductStockInvalidException(int negativeQuantity)
    {
        // Arrange
        var product = Product.Create("Laptop", "High-performance laptop", 999.99m, 10);

        // Act
        var act = () => product.DecreaseStock(negativeQuantity);

        // Assert
        act.Should().Throw<ProductStockInvalidException>();
    }
}
