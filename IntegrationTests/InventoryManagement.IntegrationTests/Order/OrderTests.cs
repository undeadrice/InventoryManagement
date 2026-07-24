using FluentAssertions;
using InventoryManagement.API.Orders.Responses;
using InventoryManagement.Application.Customers.Commands;
using InventoryManagement.Application.Orders.Commands;
using InventoryManagement.Application.Products.Commands;
using InventoryManagement.Domain.Customers;
using InventoryManagement.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InventoryManagement.IntegrationTests.Order;

public class OrderTests : IAsyncLifetime
{
    private readonly InventoryWebApplicationFactory _factory = new();
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        await _factory.CreateDatabase();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DeleteDatabase();
    }

    private async Task<Guid> CreateProductAsync(string name = "Test Product", string description = "A product", decimal price = 10.00m, int stock = 100)
    {
        var command = new CreateProductCommand(name, description, price, stock);
        var response = await _client.PostAsJsonAsync("/api/products", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateCustomerAsync(CustomerLocation location = CustomerLocation.US)
    {
        var command = new CreateCustomerCommand(location);
        var response = await _client.PostAsJsonAsync("/api/customers", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    [Fact]
    public async Task GetOrders_WhenNoOrdersExist_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
        orders.Should().NotBeNull();
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOrders_AfterCreatingMultipleOrders_ReturnsAllOrders()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();
        var productId = await CreateProductAsync("Multi-Order Product", "For multiple orders", 5.00m, 200);

        var orderIds = new List<Guid>();
        for (int i = 0; i < 3; i++)
        {
            var command = new CreateOrderCommand(customerId, [new OrderItemRequest(productId, 1)]);
            var r = await _client.PostAsJsonAsync("/api/orders", command);
            r.StatusCode.Should().Be(HttpStatusCode.OK);
            var orderId = await r.Content.ReadFromJsonAsync<Guid>();
            orderIds.Add(orderId);
        }

        // Act
        var getResponse = await _client.GetAsync("/api/orders");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var orders = await getResponse.Content.ReadFromJsonAsync<List<OrderResponse>>();
        orders.Should().NotBeNull();
        orders.Select(o => o.Id).Should().Contain(orderIds);
    }

    [Fact]
    public async Task GetOrderById_WithExistingOrder_ReturnsOrder()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();
        var productId = await CreateProductAsync("Gadget", "A cool gadget", 49.99m, 10);

        var command = new CreateOrderCommand(customerId, [new OrderItemRequest(productId, 3)]);
        var createResponse = await _client.PostAsJsonAsync("/api/orders", command);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var orderId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var getResponse = await _client.GetAsync($"/api/orders/{orderId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();
        order.Should().NotBeNull();
        order.Id.Should().Be(orderId);
        order.CustomerId.Should().Be(customerId);
        order.Items.Should().ContainSingle(i => i.ProductId == productId && i.Quantity == 3);
        order.FinalPrice.Should().BeGreaterThan(0);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetOrderById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/orders/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsOkWithGuid()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();
        var productId = await CreateProductAsync("Valid Product", "A valid product", 25.00m, 50);

        var command = new CreateOrderCommand(customerId, [new OrderItemRequest(productId, 1)]);

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var orderId = await response.Content.ReadFromJsonAsync<Guid>();
        orderId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateOrder_WithNonExistentCustomer_ReturnsNotFound()
    {
        // Arrange
        var productId = await CreateProductAsync("Some Product", "A product", 10.00m, 10);
        var nonExistentCustomerId = Guid.NewGuid();

        var command = new CreateOrderCommand(nonExistentCustomerId, [new OrderItemRequest(productId, 1)]);

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WithNonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();
        var nonExistentProductId = Guid.NewGuid();

        var command = new CreateOrderCommand(customerId, [new OrderItemRequest(nonExistentProductId, 1)]);

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ReturnsBadRequest()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();
        var productId = await CreateProductAsync("Low Stock Item", "Limited stock", 15.00m, 5);

        var command = new CreateOrderCommand(customerId, [new OrderItemRequest(productId, 10)]);

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithEmptyCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var productId = await CreateProductAsync("Any Product", "A product", 10.00m, 10);

        var command = new CreateOrderCommand(Guid.Empty, [new OrderItemRequest(productId, 1)]);

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithNoItems_ReturnsBadRequest()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();

        var command = new CreateOrderCommand(customerId, []);

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_DecreasesProductStock()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();
        var productId = await CreateProductAsync("Stock Tracker", "Track stock changes", 10.00m, 20);

        var command = new CreateOrderCommand(customerId, [new OrderItemRequest(productId, 7)]);

        // Act
        var orderResponse = await _client.PostAsJsonAsync("/api/orders", command);
        orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var overStockCommand = new CreateOrderCommand(customerId, [new OrderItemRequest(productId, 14)]);
        var overStockResponse = await _client.PostAsJsonAsync("/api/orders", overStockCommand);
        overStockResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_FinalPriceIsCalculated()
    {
        // Arrange
        var customerId = await CreateCustomerAsync();
        var productId = await CreateProductAsync("Priced Item", "Has a price", 50.00m, 10);

        var command = new CreateOrderCommand(customerId, [new OrderItemRequest(productId, 2)]);

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/orders", command);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var orderId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await _client.GetAsync($"/api/orders/{orderId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();

        // Assert
        order!.FinalPrice.Should().BeGreaterThan(0);
    }
}
