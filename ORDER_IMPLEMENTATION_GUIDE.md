# Order Flow Implementation - Inventory Management System

## Overview
This document describes the complete implementation of the Order flow in the Inventory Management System, following the CQRS pattern and maintaining consistency with the existing Product and Customer flows.

## Architecture & Design Patterns

### CQRS Implementation
The Order flow follows the Command Query Responsibility Segregation (CQRS) pattern:
- **Commands**: Handle state-changing operations (CreateOrderCommand)
- **Queries**: Handle read-only operations (GetOrdersQuery, GetOrderByIdQuery)
- **Handlers**: Process commands/queries and implement business logic

### Layered Architecture
```
┌─────────────────────────────────────────┐
│  API Layer (InventoryManagement.API)    │ Controllers, Responses, Mappings
├─────────────────────────────────────────┤
│  Application Layer (Application)        │ Commands, Queries, DTOs, Handlers
├─────────────────────────────────────────┤
│  Domain Layer (Domain)                  │ Entities, Services, Exceptions
├─────────────────────────────────────────┤
│  Persistence Layer (Persistence)        │ Repositories, Configurations, DbContext
└─────────────────────────────────────────┘
```

## Domain Layer (`InventoryManagement.Domain`)

### Entities

#### Order.cs
- **Properties**:
  - `Id`: Unique identifier (Guid)
  - `CustomerId`: Reference to customer placing the order
  - `OrderItems`: Collection of items in the order
  - `FinalPrice`: Calculated price after discounts and location adjustments
  - `CreatedAt`: Timestamp when order was created

- **Methods**:
  - `Create()`: Factory method with validation

#### OrderItem.cs
- **Properties**:
  - `Id`: Unique identifier
  - `OrderId`: Reference to parent Order
  - `ProductId`: Reference to the product
  - `Quantity`: Number of units ordered
  - `UnitPrice`: Price per unit at time of purchase

- **Methods**:
  - `Create()`: Factory method
  - `SetOrderId()`: Sets the order reference after order creation

### Domain Services

#### IOrderRepository Interface
Defines data access contracts for Order persistence:
- `Add()`: Creates new order
- `Update()`: Updates existing order
- `GetById()`: Retrieves order by ID (throws if not found)
- `FindById()`: Retrieves order by ID (nullable)
- `GetAll()`: Retrieves all orders with optional filter

#### IDiscountCalculator Interface & Implementation
Handles all discount and pricing calculations:

**Discount Types (Applied in Priority Order):**

1. **Volume-Based Discounts**:
   - 5-9 units: 10% discount
   - 10-49 units: 20% discount
   - 50+ units: 30% discount

2. **Seasonal & Promotional Discounts**:
   - Black Friday (4th Friday of November): 25% discount
   - Polish Bank Holidays: 15% discount
   - Includes: Jan 1, Jan 6, May 1, May 3, Aug 15, Nov 1, Nov 11, Dec 25, Dec 26

3. **Location-Based Pricing Adjustments**:
   - US: 1.0x (standard pricing)
   - Europe: 1.15x (+15% VAT)
   - Asia: 1.05x (+5% logistics costs)

**Business Rules**:
- Only ONE discount is applied (the highest applicable)
- Location-based adjustment is applied to base price before discount
- Final price = (Base Price × Location Multiplier) - Discount Amount

**Date Handling**:
- Current date is injectable via `Func<DateTime>` for testing
- Allows predictable discount testing

### Exceptions

- `OrderCustomerIdRequiredException`: Customer ID is required
- `OrderItemsRequiredException`: Order must contain at least one item
- `OrderFinalPriceInvalidException`: Final price cannot be negative
- `InsufficientStockException`: Stock not available for requested quantity

### Product Enhancement
Added `DecreaseStock(quantity)` method to Product entity to update inventory when order is placed.

## Application Layer (`InventoryManagement.Application`)

### Commands

#### CreateOrderCommand
```csharp
public record CreateOrderCommand(
    Guid CustomerId, 
    List<OrderItemRequest> Items) : ICommand<Guid>;
```
Returns the created Order's ID (Guid)

#### CreateOrderCommandHandler
**Responsibilities**:
1. Validates customer exists
2. Validates product availability
3. Checks sufficient stock for each product
4. Calculates discounts using DiscountCalculator
5. Decreases product stock
6. Creates Order with OrderItems
7. Persists order to database

**Business Logic**:
- Throws `InsufficientStockException` if stock unavailable
- Calculates final price with all applicable discounts
- Updates product inventory atomically with order creation
- Uses Unit of Work pattern for transactional consistency

### Queries

#### GetOrdersQuery & GetOrderByIdQuery
```csharp
public record GetOrdersQuery() : IRequest<IReadOnlyCollection<OrderDto>>;
public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
```

#### Query Handlers
- `GetOrdersQueryHandler`: Returns all orders
- `GetOrderByIdQueryHandler`: Returns specific order by ID

### DTOs

#### OrderDto & OrderItemDto
```csharp
public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
public record OrderDto(
    Guid Id, 
    Guid CustomerId, 
    List<OrderItemDto> Items, 
    decimal FinalPrice, 
    DateTime CreatedAt);
```

### Mappings

#### OrderMappingExtensions
```csharp
public static OrderDto MapToOrderDto(this Order model)
```
Converts domain Order entity to application DTO.

## Persistence Layer (`InventoryManagement.Persistence`)

### Entity Configurations

#### OrderConfiguration
Configures Order table mapping:
- Primary key: Id
- Foreign key relationships with OrderItems (Cascade delete)
- Precision: FinalPrice (18,2)

#### OrderItemConfiguration
Configures OrderItem table mapping:
- Primary key: Id
- Foreign key to Order (OrderId)
- Product reference (ProductId)

### Repository Implementation

#### OrderRepository
Implements `IOrderRepository` interface:
- Uses EF Core with Include() for OrderItems
- Handles NotFoundException when order not found
- Supports filtering with Lambda expressions

### Database Context

#### PersistenceDbContext
Added DbSets:
- `DbSet<Order> Orders`
- `DbSet<OrderItem> OrderItems`

Automatically applies configurations from assembly via `ApplyConfigurationsFromAssembly()`.

### Dependency Injection

Updated `DIRegistrations.cs`:
```csharp
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IDiscountCalculator, DiscountCalculator>();
```

## API Layer (`InventoryManagement.API`)

### OrderController

Endpoints:

#### POST /api/orders
Creates a new order.
```csharp
public record OrderItemRequest(Guid ProductId, int Quantity);
POST /api/orders
{
  "customerId": "guid",
  "items": [
    {
      "productId": "guid",
      "quantity": 5
    }
  ]
}
```
**Response**: `{ "id": "guid" }`
**Status**: 200 OK
**Errors**:
- 400: Invalid product ID or insufficient stock
- 404: Customer not found

#### GET /api/orders
Retrieves all orders.
**Response**: Array of OrderResponse objects
**Status**: 200 OK

#### GET /api/orders/{orderId}
Retrieves specific order by ID.
**Response**: OrderResponse object
**Status**: 200 OK
**Errors**:
- 404: Order not found

### Response Objects

#### OrderResponse & OrderItemResponse
```csharp
public record OrderItemResponse(Guid ProductId, int Quantity, decimal UnitPrice);
public record OrderResponse(
    Guid Id, 
    Guid CustomerId, 
    List<OrderItemResponse> Items, 
    decimal FinalPrice, 
    DateTime CreatedAt);
```

### Mappings

#### OrderMappingExtensions
```csharp
public static OrderResponse MapToOrderResponse(this OrderDto dto)
```
Converts application DTO to API response object.

## Program Configuration

Updated `Program.cs` to register services:
```csharp
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
```

## Database Schema

### Orders Table
```sql
CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    FinalPrice DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
```

### OrderItems Table
```sql
CREATE TABLE OrderItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
);
```

## Business Logic Flow

### Order Creation Flow

```
POST /api/orders
    ↓
CreateOrderCommand
    ↓
CreateOrderCommandHandler.Handle()
    ├─ Verify Customer exists
    ├─ For each OrderItem:
    │   ├─ Get Product
    │   ├─ Validate Stock ≥ Quantity
    │   └─ Decrease Stock
    ├─ Calculate DiscountCalculator.CalculateDiscount()
    │   ├─ Determine Volume Discount
    │   ├─ Check Seasonal/Promotional Discounts
    │   ├─ Apply Location-based Adjustment
    │   └─ Return final price with highest discount
    ├─ Create Order entity
    ├─ Set OrderIds for OrderItems
    ├─ Save via OrderRepository.Add()
    └─ Return Order.Id
```

### Discount Calculation Flow

```
CalculateDiscount(basePrice, quantity, location, date)
    ├─ Get Volume Discount based on quantity
    ├─ Get Seasonal Discount based on date
    ├─ Select highest discount (not combined)
    ├─ Apply Location Multiplier to base price
    ├─ Calculate discount amount
    └─ Return adjusted price
```

## Key Features

✅ **CQRS Pattern**: Separate command and query responsibilities
✅ **Transaction Safety**: Unit of Work pattern ensures atomic operations
✅ **Inventory Management**: Stock decreases with each order
✅ **Complex Discount Logic**: Volume, seasonal, and promotional discounts
✅ **Location-Based Pricing**: VAT and logistics adjustments per region
✅ **Validation**: Customer and product existence verified before order
✅ **Error Handling**: Specific domain exceptions for business rule violations
✅ **Testable Design**: Discount calculator accepts injectable date function
✅ **Clean Architecture**: Proper separation of concerns across layers
✅ **Consistent Patterns**: Follows Product and Customer flow structure

## Testing Considerations

### Test Scenarios for Discount Calculator
- Volume-based discounts (5, 10, 50+ units)
- Seasonal discounts (Black Friday, Polish holidays)
- Location-based pricing (US, Europe, Asia)
- Discount priority (only highest applied)
- Combination of volume + seasonal + location

### Test Scenarios for Order Creation
- Valid order creation
- Insufficient stock handling
- Non-existent customer validation
- Non-existent product validation
- Stock updates after order
- Empty items validation

### Integration Tests
- Full order creation flow
- Database persistence
- Transaction rollback on error
- Multiple products in single order
- Discount calculations with real dates

## Example Usage

### Creating an Order
```csharp
var command = new CreateOrderCommand(
    customerId: new Guid("..."),
    items: new List<OrderItemRequest>
    {
        new OrderItemRequest(productId: new Guid("..."), quantity: 5),
        new OrderItemRequest(productId: new Guid("..."), quantity: 3)
    }
);

var orderId = await mediator.Send(command);
```

### Retrieving Orders
```csharp
var allOrders = await mediator.Send(new GetOrdersQuery());
var specificOrder = await mediator.Send(new GetOrderByIdQuery(orderId));
```

## Future Enhancements

1. **Order Status Tracking**: Add status field (Pending, Processing, Shipped, Delivered)
2. **Order Cancellation**: Implement command to cancel orders and restore stock
3. **Payment Processing**: Add payment status and payment method tracking
4. **Shipping Information**: Add address and shipping details
5. **Order History/Timeline**: Track status changes with timestamps
6. **Bulk Discounts**: More sophisticated pricing rules for wholesale
7. **Customer Loyalty**: Customer tier-based discounts
8. **Real-time Notifications**: Event-driven notifications for order status
9. **Analytics**: Order trends, revenue reports, popular products

## Consistency with Existing Patterns

This implementation maintains complete consistency with the Product and Customer flows:
- Same CQRS structure (Command, Query, Handler, DTO)
- Same layer organization (API, Application, Domain, Persistence)
- Same mapping patterns (Mapping extensions for DTO and Response conversion)
- Same repository pattern (IRepository with standard CRUD methods)
- Same exception handling (Domain exceptions for business rules)
- Same transaction management (Unit of Work pattern via TransactionBehavior)
- Same dependency injection approach (DIRegistrations with extension methods)
