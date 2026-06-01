using FluentAssertions;
using InventoryManagement.API.Products.Responses;
using InventoryManagement.Application.Products.Commands;
using InventoryManagement.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InventoryManagement.IntegrationTests.Product;

public class ProductTests : IClassFixture<InventoryWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductTests(InventoryWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -------------------------------------------------------------------------
    // GET /api/product
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetProducts_WhenNoProductsExist_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/product");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
        products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_AfterCreatingProduct_ReturnsProductInList()
    {
        // Arrange – create a product first
        var command = new CreateProductCommand("Widget", "A small widget", 9.99m, 100);
        var createResponse = await _client.PostAsJsonAsync("/api/product", command);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var getResponse = await _client.GetAsync("/api/product");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await getResponse.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
        products.Should().ContainSingle(p =>
            p.Name == "Widget" &&
            p.Descrption == "A small widget" &&
            p.Price == 9.99m &&
            p.Stock == 100);
    }

    [Fact]
    public async Task GetProducts_AfterCreatingMultipleProducts_ReturnsAllProducts()
    {
        // Arrange
        var commands = new[]
        {
            new CreateProductCommand("Alpha", "First product", 1.00m, 10),
            new CreateProductCommand("Beta",  "Second product", 2.00m, 20),
            new CreateProductCommand("Gamma", "Third product",  3.00m, 30),
        };

        foreach (var cmd in commands)
        {
            var r = await _client.PostAsJsonAsync("/api/product", cmd);
            r.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Act
        var getResponse = await _client.GetAsync("/api/product");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await getResponse.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
        products!.Select(p => p.Name).Should().Contain(["Alpha", "Beta", "Gamma"]);
    }

    // -------------------------------------------------------------------------
    // POST /api/product
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsOkWithGuid()
    {
        // Arrange
        var command = new CreateProductCommand("Gadget", "A cool gadget", 49.99m, 50);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProduct_WithZeroPrice_ReturnsBadRequest()
    {
        // Arrange – price must be > 0
        var command = new CreateProductCommand("Freebie", "Zero price item", 0m, 10);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithNegativePrice_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateProductCommand("Negative", "Negative price item", -5.00m, 10);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithNegativeStock_ReturnsBadRequest()
    {
        // Arrange – stock must be >= 0
        var command = new CreateProductCommand("Overdrawn", "Negative stock item", 10.00m, -1);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateProductCommand("", "Some description", 10.00m, 5);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithNameExceeding50Characters_ReturnsBadRequest()
    {
        // Arrange – name max length is 50
        var longName = new string('A', 51);
        var command = new CreateProductCommand(longName, "Valid description", 10.00m, 5);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithEmptyDescription_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateProductCommand("Valid Name", "", 10.00m, 5);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithDescriptionExceeding50Characters_ReturnsBadRequest()
    {
        // Arrange – description max length is 50
        var longDescription = new string('D', 51);
        var command = new CreateProductCommand("Valid Name", longDescription, 10.00m, 5);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WithZeroStock_ReturnsOk()
    {
        // Arrange – stock of 0 is valid
        var command = new CreateProductCommand("Out of Stock Item", "No stock yet", 5.00m, 0);

        // Act
        var response = await _client.PostAsJsonAsync("/api/product", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
    }
}
