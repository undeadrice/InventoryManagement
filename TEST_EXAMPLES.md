# Order Implementation - Test Examples

This document provides example test cases for validating the Order flow implementation.

## Unit Tests for DiscountCalculator

### Test: Volume-Based Discount - 5 Units
```csharp
[Fact]
public void CalculateDiscount_With5Units_ReturnsBasePrice()
{
    // Arrange
    var calculator = new DiscountCalculator();
    decimal basePrice = 100m;
    int quantity = 5;
    var location = CustomerLocation.US;
    var date = DateTime.Now;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, date);

    // Assert - 10% discount on 5 units: 100 * 0.9 = 90
    Assert.Equal(90m, result);
}
```

### Test: Volume-Based Discount - 10 Units
```csharp
[Fact]
public void CalculateDiscount_With10Units_Returns20PercentDiscount()
{
    // Arrange
    var calculator = new DiscountCalculator();
    decimal basePrice = 100m;
    int quantity = 10;
    var location = CustomerLocation.US;
    var date = DateTime.Now;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, date);

    // Assert - 20% discount on 10 units: 100 * 0.8 = 80
    Assert.Equal(80m, result);
}
```

### Test: Volume-Based Discount - 50 Units
```csharp
[Fact]
public void CalculateDiscount_With50Units_Returns30PercentDiscount()
{
    // Arrange
    var calculator = new DiscountCalculator();
    decimal basePrice = 100m;
    int quantity = 50;
    var location = CustomerLocation.US;
    var date = DateTime.Now;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, date);

    // Assert - 30% discount on 50 units: 100 * 0.7 = 70
    Assert.Equal(70m, result);
}
```

### Test: Black Friday Discount
```csharp
[Fact]
public void CalculateDiscount_OnBlackFriday_Returns25PercentDiscount()
{
    // Arrange - 4th Friday of November 2024
    var blackFridayDate = new DateTime(2024, 11, 22);
    Func<DateTime> getDate = () => blackFridayDate;
    var calculator = new DiscountCalculator(getDate);

    decimal basePrice = 100m;
    int quantity = 2; // Below 5, no volume discount
    var location = CustomerLocation.US;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, blackFridayDate);

    // Assert - 25% discount on Black Friday: 100 * 0.75 = 75
    Assert.Equal(75m, result);
}
```

### Test: Polish Holiday Discount
```csharp
[Fact]
public void CalculateDiscount_OnPolishHoliday_Returns15PercentDiscount()
{
    // Arrange - January 1st (New Year's Day)
    var holidayDate = new DateTime(2024, 1, 1);
    Func<DateTime> getDate = () => holidayDate;
    var calculator = new DiscountCalculator(getDate);

    decimal basePrice = 100m;
    int quantity = 2; // Below 5, no volume discount
    var location = CustomerLocation.US;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, holidayDate);

    // Assert - 15% discount on holiday: 100 * 0.85 = 85
    Assert.Equal(85m, result);
}
```

### Test: Europe Location (15% VAT)
```csharp
[Fact]
public void CalculateDiscount_EuropeLocation_AppliesVAT()
{
    // Arrange
    var calculator = new DiscountCalculator();
    decimal basePrice = 100m;
    int quantity = 1; // No volume discount
    var location = CustomerLocation.EUROPE;
    var date = DateTime.Now;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, date);

    // Assert - 15% VAT: 100 * 1.15 = 115
    Assert.Equal(115m, result);
}
```

### Test: Asia Location (5% Logistics)
```csharp
[Fact]
public void CalculateDiscount_AsiaLocation_AppliesLogisticsCost()
{
    // Arrange
    var calculator = new DiscountCalculator();
    decimal basePrice = 100m;
    int quantity = 1; // No volume discount
    var location = CustomerLocation.ASIA;
    var date = DateTime.Now;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, date);

    // Assert - 5% logistics: 100 * 1.05 = 105
    Assert.Equal(105m, result);
}
```

### Test: Discount Priority (Highest Discount Wins)
```csharp
[Fact]
public void CalculateDiscount_Volume30AndBlackFriday_Returns30VolumeDiscount()
{
    // Arrange - 4th Friday of November with 50 units
    var blackFridayDate = new DateTime(2024, 11, 22);
    Func<DateTime> getDate = () => blackFridayDate;
    var calculator = new DiscountCalculator(getDate);

    decimal basePrice = 100m;
    int quantity = 50; // Eligible for 30% volume discount
    var location = CustomerLocation.US;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, blackFridayDate);

    // Assert - 30% volume discount (higher than 25% Black Friday): 100 * 0.7 = 70
    Assert.Equal(70m, result);
}
```

### Test: Combined Location + Volume Discount
```csharp
[Fact]
public void CalculateDiscount_Europe10Units_AppliesVATThenVolumeDiscount()
{
    // Arrange
    var calculator = new DiscountCalculator();
    decimal basePrice = 100m;
    int quantity = 10; // 20% volume discount
    var location = CustomerLocation.EUROPE; // 15% VAT
    var date = DateTime.Now;

    // Act
    var result = calculator.CalculateDiscount(basePrice, quantity, location, date);

    // Assert
    // Step 1: Apply location multiplier: 100 * 1.15 = 115
    // Step 2: Apply discount: 115 * 0.8 = 92
    Assert.Equal(92m, result);
}
```

## Unit Tests for Order Entity

### Test: Order Creation with Valid Data
```csharp
[Fact]
public void Create_WithValidData_ReturnsOrderWithCorrectValues()
{
    // Arrange
    var customerId = Guid.NewGuid();
    var items = new List<OrderItem>
    {
        OrderItem.Create(Guid.NewGuid(), 5, 100m)
    };
    decimal finalPrice = 450m;

    // Act
    var order = Order.Create(customerId, items, finalPrice);

    // Assert
    Assert.NotEqual(Guid.Empty, order.Id);
    Assert.Equal(customerId, order.CustomerId);
    Assert.Single(order.OrderItems);
    Assert.Equal(finalPrice, order.FinalPrice);
    Assert.NotEqual(DateTime.MinValue, order.CreatedAt);
}
```

### Test: Order Creation with Empty Customer ID
```csharp
[Fact]
public void Create_WithEmptyCustomerId_ThrowsOrderCustomerIdRequiredException()
{
    // Arrange
    var items = new List<OrderItem>
    {
        OrderItem.Create(Guid.NewGuid(), 5, 100m)
    };

    // Act & Assert
    Assert.Throws<OrderCustomerIdRequiredException>(
        () => Order.Create(Guid.Empty, items, 450m));
}
```

### Test: Order Creation with No Items
```csharp
[Fact]
public void Create_WithNoItems_ThrowsOrderItemsRequiredException()
{
    // Act & Assert
    Assert.Throws<OrderItemsRequiredException>(
        () => Order.Create(Guid.NewGuid(), new List<OrderItem>(), 100m));
}
```

### Test: Order Creation with Negative Price
```csharp
[Fact]
public void Create_WithNegativePrice_ThrowsOrderFinalPriceInvalidException()
{
    // Arrange
    var items = new List<OrderItem>
    {
        OrderItem.Create(Guid.NewGuid(), 5, 100m)
    };

    // Act & Assert
    Assert.Throws<OrderFinalPriceInvalidException>(
        () => Order.Create(Guid.NewGuid(), items, -50m));
}
```

## Integration Tests for CreateOrderCommandHandler

### Test: Valid Order Creation
```csharp
[Fact]
public async Task Handle_WithValidCommand_CreatesOrderAndDecrementsStock()
{
    // Arrange
    var customerId = Guid.NewGuid();
    var productId = Guid.NewGuid();

    var customer = Customer.Create(CustomerLocation.US);
    var product = Product.Create("Test Product", "Description", 100m, 10);

    var command = new CreateOrderCommand(
        customerId,
        new List<OrderItemRequest>
        {
            new OrderItemRequest(productId, 5)
        });

    // Mock repositories...
    var mockProductRepo = new Mock<IProductRepository>();
    var mockCustomerRepo = new Mock<ICustomerRepository>();
    var mockOrderRepo = new Mock<IOrderRepository>();
    var mockCalculator = new Mock<IDiscountCalculator>();

    mockCustomerRepo
        .Setup(x => x.GetById(customerId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(customer);

    mockProductRepo
        .Setup(x => x.GetById(productId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(product);

    mockCalculator
        .Setup(x => x.CalculateDiscount(
            It.IsAny<decimal>(),
            It.IsAny<int>(),
            It.IsAny<CustomerLocation>(),
            It.IsAny<DateTime>()))
        .Returns(450m); // 5 units * 100 = 500, 10% discount = 450

    var handler = new CreateOrderCommandHandler(
        mockOrderRepo.Object,
        mockProductRepo.Object,
        mockCustomerRepo.Object,
        mockCalculator.Object);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.NotEqual(Guid.Empty, result);
    mockOrderRepo.Verify(x => x.Add(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    mockProductRepo.Verify(x => x.Update(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Test: Order with Insufficient Stock
```csharp
[Fact]
public async Task Handle_WithInsufficientStock_ThrowsInsufficientStockException()
{
    // Arrange
    var customerId = Guid.NewGuid();
    var productId = Guid.NewGuid();

    var customer = Customer.Create(CustomerLocation.US);
    var product = Product.Create("Test Product", "Description", 100m, 3); // Only 3 in stock

    var command = new CreateOrderCommand(
        customerId,
        new List<OrderItemRequest>
        {
            new OrderItemRequest(productId, 5) // Requesting 5
        });

    var mockProductRepo = new Mock<IProductRepository>();
    var mockCustomerRepo = new Mock<ICustomerRepository>();
    var mockOrderRepo = new Mock<IOrderRepository>();
    var mockCalculator = new Mock<IDiscountCalculator>();

    mockCustomerRepo
        .Setup(x => x.GetById(customerId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(customer);

    mockProductRepo
        .Setup(x => x.GetById(productId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(product);

    var handler = new CreateOrderCommandHandler(
        mockOrderRepo.Object,
        mockProductRepo.Object,
        mockCustomerRepo.Object,
        mockCalculator.Object);

    // Act & Assert
    await Assert.ThrowsAsync<InsufficientStockException>(
        () => handler.Handle(command, CancellationToken.None));
}
```

### Test: Order with Non-Existent Customer
```csharp
[Fact]
public async Task Handle_WithNonExistentCustomer_ThrowsNotFoundException()
{
    // Arrange
    var customerId = Guid.NewGuid();
    var productId = Guid.NewGuid();

    var command = new CreateOrderCommand(
        customerId,
        new List<OrderItemRequest>
        {
            new OrderItemRequest(productId, 5)
        });

    var mockProductRepo = new Mock<IProductRepository>();
    var mockCustomerRepo = new Mock<ICustomerRepository>();
    var mockOrderRepo = new Mock<IOrderRepository>();
    var mockCalculator = new Mock<IDiscountCalculator>();

    mockCustomerRepo
        .Setup(x => x.GetById(customerId, It.IsAny<CancellationToken>()))
        .ThrowsAsync(new NotFoundException($"Customer with id {customerId} doesn't exist"));

    var handler = new CreateOrderCommandHandler(
        mockOrderRepo.Object,
        mockProductRepo.Object,
        mockCustomerRepo.Object,
        mockCalculator.Object);

    // Act & Assert
    await Assert.ThrowsAsync<NotFoundException>(
        () => handler.Handle(command, CancellationToken.None));
}
```

## API Integration Tests

### Test: POST /api/orders - Success
```csharp
[Fact]
public async Task CreateOrder_WithValidCommand_ReturnsOkWithOrderId()
{
    // Arrange - Setup using WebApplicationFactory
    var client = _factory.CreateClient();
    var command = new CreateOrderCommand(
        Guid.NewGuid(),
        new List<OrderItemRequest>
        {
            new OrderItemRequest(Guid.NewGuid(), 5)
        });

    var content = new StringContent(
        JsonConvert.SerializeObject(command),
        Encoding.UTF8,
        "application/json");

    // Act
    var response = await client.PostAsync("/api/orders", content);

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    var responseContent = await response.Content.ReadAsStringAsync();
    responseContent.Should().Contain("id");
}
```

### Test: GET /api/orders - Returns All Orders
```csharp
[Fact]
public async Task GetOrders_ReturnsOkWithAllOrders()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/orders");

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    var content = await response.Content.ReadAsStringAsync();
    var orders = JsonConvert.DeserializeObject<List<OrderResponse>>(content);
    orders.Should().NotBeNull();
}
```

### Test: GET /api/orders/{orderId} - Returns Specific Order
```csharp
[Fact]
public async Task GetOrderById_WithValidId_ReturnsOkWithOrder()
{
    // Arrange
    var client = _factory.CreateClient();
    var orderId = Guid.NewGuid();

    // Act
    var response = await client.GetAsync($"/api/orders/{orderId}");

    // Assert - Depends on whether order exists
    // Success case:
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
}
```

## Test Data Builders

### OrderBuilder for Creating Test Orders
```csharp
public class OrderBuilder
{
    private Guid _customerId = Guid.NewGuid();
    private List<OrderItem> _items = new();
    private decimal _finalPrice = 0;

    public OrderBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public OrderBuilder AddItem(Guid productId, int quantity, decimal price)
    {
        _items.Add(OrderItem.Create(productId, quantity, price));
        _finalPrice += quantity * price;
        return this;
    }

    public OrderBuilder WithFinalPrice(decimal price)
    {
        _finalPrice = price;
        return this;
    }

    public Order Build()
    {
        return Order.Create(_customerId, _items, _finalPrice);
    }
}

// Usage:
var order = new OrderBuilder()
    .WithCustomerId(customerId)
    .AddItem(productId1, 5, 100)
    .AddItem(productId2, 3, 50)
    .WithFinalPrice(450)
    .Build();
```

## Performance Test Example

### Test: Order Creation with Large Item Count
```csharp
[Fact]
public async Task CreateOrder_With100Items_CompletesWithinTimeLimit()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();
    var items = Enumerable.Range(0, 100)
        .Select(i => new OrderItemRequest(Guid.NewGuid(), 1))
        .ToList();

    var command = new CreateOrderCommand(Guid.NewGuid(), items);

    // Act
    await _handler.Handle(command, CancellationToken.None);

    stopwatch.Stop();

    // Assert - Should complete within 1 second
    Assert.True(stopwatch.ElapsedMilliseconds < 1000,
        $"Order creation took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
}
```

## Test Coverage Summary

- ✅ Discount Calculator (8+ unit tests)
- ✅ Order Entity Validation (4+ unit tests)
- ✅ Order Creation Handler (3+ integration tests)
- ✅ API Endpoints (3+ integration tests)
- ✅ Error Scenarios (4+ exception tests)
- ✅ Business Rules (5+ business logic tests)

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=DiscountCalculatorTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```
