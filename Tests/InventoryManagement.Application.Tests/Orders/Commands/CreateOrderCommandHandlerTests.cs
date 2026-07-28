using FluentAssertions;
using InventoryManagement.Application.Auth.Services;
using InventoryManagement.Application.Orders.Commands;
using InventoryManagement.Domain.Customers;
using InventoryManagement.Domain.Customers.Services;
using InventoryManagement.Domain.Orders.Entities;
using InventoryManagement.Domain.Orders.Exceptions;
using InventoryManagement.Domain.Orders.Services;
using InventoryManagement.Domain.Products.Entities;
using InventoryManagement.Domain.Products.Services;
using InventoryManagement.Shared.Exceptions;
using NSubstitute;
using Xunit;

namespace InventoryManagement.Application.Tests.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IDiscountCalculator _discountCalculator;
    private readonly ICurrentUserService _currentUserService;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _customerRepository = Substitute.For<ICustomerRepository>();
        _discountCalculator = Substitute.For<IDiscountCalculator>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _currentUserService.CurrentUserId.Returns(Guid.NewGuid());
        _handler = new CreateOrderCommandHandler(
            _orderRepository,
            _productRepository,
            _customerRepository,
            _discountCalculator,
            _currentUserService);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddOrderToRepositoryReturnOrderIdAndDecreaseProductStock()
    {
        // Arrange
        var customer = Customer.Create(CustomerLocation.US);
        var product = Product.Create("Laptop", "High-end laptop", 999.99m, 10);
        var command = new CreateOrderCommand(customer.Id, [new OrderItemRequest(product.Id, 3)]);

        _customerRepository.GetById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _productRepository.GetById(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _discountCalculator
            .CalculateDiscount(Arg.Any<IEnumerable<OrderLineItem>>(), Arg.Any<CustomerLocation>(), Arg.Any<DateTime>())
            .Returns(2999.97m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        await _orderRepository.Received(1).Add(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        product.Stock.Should().Be(7);
        await _productRepository.Received(1).Update(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithMultipleItems_ShouldUpdateEachProductAndAddOrder()
    {
        // Arrange
        var customer = Customer.Create(CustomerLocation.US);
        var product1 = Product.Create("Laptop", "High-end laptop", 999.99m, 10);
        var product2 = Product.Create("Mouse", "Wireless mouse", 49.99m, 20);
        var command = new CreateOrderCommand(customer.Id,
        [
            new OrderItemRequest(product1.Id, 1),
            new OrderItemRequest(product2.Id, 2)
        ]);

        _customerRepository.GetById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _productRepository.GetById(product1.Id, Arg.Any<CancellationToken>()).Returns(product1);
        _productRepository.GetById(product2.Id, Arg.Any<CancellationToken>()).Returns(product2);
        _discountCalculator
            .CalculateDiscount(Arg.Any<IEnumerable<OrderLineItem>>(), Arg.Any<CustomerLocation>(), Arg.Any<DateTime>())
            .Returns(1099.97m);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _productRepository.Received(1).Update(product1, Arg.Any<CancellationToken>());
        await _productRepository.Received(1).Update(product2, Arg.Any<CancellationToken>());
        await _orderRepository.Received(1).Add(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPassCorrectLineItemsAndLocationToDiscountCalculator()
    {
        // Arrange
        var customer = Customer.Create(CustomerLocation.EUROPE);
        var product = Product.Create("Laptop", "High-end laptop", 100m, 10);
        var command = new CreateOrderCommand(customer.Id, [new OrderItemRequest(product.Id, 5)]);

        _customerRepository.GetById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _productRepository.GetById(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _discountCalculator
            .CalculateDiscount(Arg.Any<IEnumerable<OrderLineItem>>(), Arg.Any<CustomerLocation>(), Arg.Any<DateTime>())
            .Returns(500m);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _discountCalculator.Received(1).CalculateDiscount(
            Arg.Is<IEnumerable<OrderLineItem>>(items =>
                items.Single().UnitPrice == 100m && items.Single().Quantity == 5),
            CustomerLocation.EUROPE,
            Arg.Any<DateTime>());
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSetOrderIdOnAllOrderItems()
    {
        // Arrange
        var customer = Customer.Create(CustomerLocation.US);
        var product = Product.Create("Laptop", "High-end laptop", 999.99m, 10);
        var command = new CreateOrderCommand(customer.Id, [new OrderItemRequest(product.Id, 1)]);

        _customerRepository.GetById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _productRepository.GetById(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _discountCalculator
            .CalculateDiscount(Arg.Any<IEnumerable<OrderLineItem>>(), Arg.Any<CustomerLocation>(), Arg.Any<DateTime>())
            .Returns(999.99m);

        Order? capturedOrder = null;
        await _orderRepository.Add(Arg.Do<Order>(o => capturedOrder = o), Arg.Any<CancellationToken>());

        // Act
        var orderId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedOrder.Should().NotBeNull();
        capturedOrder!.OrderItems.Should().AllSatisfy(item => item.OrderId.Should().Be(orderId));
    }

    [Fact]
    public async Task Handle_WhenProductHasInsufficientStock_ShouldThrowInsufficientStockExceptionAndNotAddOrder()
    {
        // Arrange
        var customer = Customer.Create(CustomerLocation.US);
        var product = Product.Create("Laptop", "High-end laptop", 999.99m, 2);
        var command = new CreateOrderCommand(customer.Id, [new OrderItemRequest(product.Id, 5)]);

        _customerRepository.GetById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _productRepository.GetById(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InsufficientStockException>();
        await _orderRepository.DidNotReceive().Add(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCustomerNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand(customerId, [new OrderItemRequest(Guid.NewGuid(), 1)]);

        _customerRepository.GetById(customerId, Arg.Any<CancellationToken>())
            .Returns<Customer>(_ => throw new NotFoundException($"Customer with id {customerId} not found"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var customer = Customer.Create(CustomerLocation.US);
        var productId = Guid.NewGuid();
        var command = new CreateOrderCommand(customer.Id, [new OrderItemRequest(productId, 1)]);

        _customerRepository.GetById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _productRepository.GetById(productId, Arg.Any<CancellationToken>())
            .Returns<Product>(_ => throw new NotFoundException($"Product with id {productId} not found"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithExactStockQuantity_ShouldSucceed()
    {
        // Arrange
        var customer = Customer.Create(CustomerLocation.US);
        var product = Product.Create("Laptop", "High-end laptop", 999.99m, 5);
        var command = new CreateOrderCommand(customer.Id, [new OrderItemRequest(product.Id, 5)]);

        _customerRepository.GetById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        _productRepository.GetById(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _discountCalculator
            .CalculateDiscount(Arg.Any<IEnumerable<OrderLineItem>>(), Arg.Any<CustomerLocation>(), Arg.Any<DateTime>())
            .Returns(4999.95m);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        product.Stock.Should().Be(0);
    }
}
