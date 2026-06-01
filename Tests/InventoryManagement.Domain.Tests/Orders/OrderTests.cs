using FluentAssertions;
using InventoryManagement.Domain.Orders;
using InventoryManagement.Domain.Orders.Exceptions;
using Xunit;

namespace InventoryManagement.Domain.Tests.Orders;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var finalPrice = 200m;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(Guid.NewGuid(), 2, 50m),
            OrderItem.Create(Guid.NewGuid(), 1, 100m)
        };
 

        // Act
        var order = Order.Create(customerId, orderItems, finalPrice);

        // Assert
        order.Should().NotBeNull();
        order.Id.Should().NotBe(Guid.Empty);
        order.CustomerId.Should().Be(customerId);
        order.OrderItems.Should().HaveCount(2);
        order.FinalPrice.Should().Be(finalPrice);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ShouldThrowOrderCustomerIdRequiredException()
    {
        // Arrange
        var orderItems = new List<OrderItem> { OrderItem.Create(Guid.NewGuid(), 1, 50m) };

        // Act
        var act = () => Order.Create(Guid.Empty, orderItems, 50m);

        // Assert
        act.Should().Throw<OrderCustomerIdRequiredException>();
    }

    [Fact]
    public void Create_WithNullOrderItems_ShouldThrowOrderItemsRequiredException()
    {
        // Act
        var act = () => Order.Create(Guid.NewGuid(), null!, 50m);

        // Assert
        act.Should().Throw<OrderItemsRequiredException>();
    }

    [Fact]
    public void Create_WithEmptyOrderItems_ShouldThrowOrderItemsRequiredException()
    {
        // Act
        var act = () => Order.Create(Guid.NewGuid(), new List<OrderItem>(), 50m);

        // Assert
        act.Should().Throw<OrderItemsRequiredException>();
    }

    [Fact]
    public void Create_WithNegativeFinalPrice_ShouldThrowOrderFinalPriceInvalidException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderItems = new List<OrderItem> { OrderItem.Create(Guid.NewGuid(), 1, 50m) };

        // Act
        var act = () => Order.Create(customerId, orderItems, -10m);

        // Assert
        act.Should().Throw<OrderFinalPriceInvalidException>();
    }

    [Fact]
    public void Create_WithZeroFinalPrice_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderItems = new List<OrderItem> { OrderItem.Create(Guid.NewGuid(), 1, 50m) };

        // Act
        var order = Order.Create(customerId, orderItems, 0m);

        // Assert
        order.Should().NotBeNull();
        order.FinalPrice.Should().Be(0m);
    }

    [Fact]
    public void Create_WithMultipleOrderItems_ShouldContainAllItems()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var item1 = OrderItem.Create(Guid.NewGuid(), 2, 50m);
        var item2 = OrderItem.Create(Guid.NewGuid(), 3, 30m);
        var item3 = OrderItem.Create(Guid.NewGuid(), 1, 100m);
        var orderItems = new List<OrderItem> { item1, item2, item3 };

        // Act
        var order = Order.Create(customerId, orderItems, 300m);

        // Assert
        order.OrderItems.Should().HaveCount(3);
        order.OrderItems.Should().Contain(item1);
        order.OrderItems.Should().Contain(item2);
        order.OrderItems.Should().Contain(item3);
    }
}
