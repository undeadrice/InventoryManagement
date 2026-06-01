# Order Implementation - Architecture Diagrams

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT APPLICATION                      │
│                    (Web Browser, Mobile App, etc.)               │
└──────────────────────────────┬──────────────────────────────────┘
                               │
                    HTTP Requests/Responses
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│                        API LAYER                                │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                  OrderController                            │ │
│  │  ┌─────────────────────────────────────────────────────┐  │ │
│  │  │ POST /api/orders → CreateOrderCommand              │  │ │
│  │  │ GET /api/orders → GetOrdersQuery                   │  │ │
│  │  │ GET /api/orders/{id} → GetOrderByIdQuery           │  │ │
│  │  └─────────────────────────────────────────────────────┘  │ │
│  │                         ↓                                  │ │
│  │  Mappers: OrderResponse (Response Objects)                │ │
│  └────────────────────────────────────────────────────────────┘ │
└──────────────────────────────┬──────────────────────────────────┘
                               │
                    MediatR Send(Command/Query)
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│                   APPLICATION LAYER                             │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │ Commands/Queries & Handlers                                │ │
│  │                                                             │ │
│  │ CreateOrderCommand                                          │ │
│  │    ↓                                                        │ │
│  │ CreateOrderCommandHandler                                  │ │
│  │    • Validates customer exists                             │ │
│  │    • Validates products exist                              │ │
│  │    • Checks stock availability                             │ │
│  │    • Calculates discounts                                  │ │
│  │    • Calls IOrderRepository.Add()                          │ │
│  │                                                             │ │
│  │ GetOrdersQuery → GetOrdersQueryHandler                     │ │
│  │ GetOrderByIdQuery → GetOrderByIdQueryHandler               │ │
│  │                                                             │ │
│  │ DTOs: OrderDto, OrderItemDto (Data Transfer Objects)       │ │
│  └────────────────────────────────────────────────────────────┘ │
└──────────────────────────────┬──────────────────────────────────┘
                               │
                 IOrderRepository (Dependency)
                 IDiscountCalculator (Dependency)
                 IProductRepository (Dependency)
                 ICustomerRepository (Dependency)
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│                       DOMAIN LAYER                              │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │ Entities:                                                  │ │
│  │  • Order (aggregate root)                                  │ │
│  │  • OrderItem                                               │ │
│  │                                                             │ │
│  │ Services:                                                  │ │
│  │  • IOrderRepository (contract)                             │ │
│  │  • IDiscountCalculator (contract)                          │ │
│  │  • DiscountCalculator (implementation)                     │ │
│  │                                                             │ │
│  │ Exceptions:                                                │ │
│  │  • OrderCustomerIdRequiredException                        │ │
│  │  • OrderItemsRequiredException                             │ │
│  │  • OrderFinalPriceInvalidException                         │ │
│  │  • InsufficientStockException                              │ │
│  │                                                             │ │
│  │ Business Rules Enforced:                                   │ │
│  │  • Customer exists                                         │ │
│  │  • Stock sufficient                                        │ │
│  │  • Final price valid                                       │ │
│  └────────────────────────────────────────────────────────────┘ │
└──────────────────────────────┬──────────────────────────────────┘
                               │
         Implemented by OrderRepository, DiscountCalculator
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│                    PERSISTENCE LAYER                            │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │ OrderRepository                                             │ │
│  │  • Add(order)                                               │ │
│  │  • Update(order)                                            │ │
│  │  • GetById(id)                                              │ │
│  │  • FindById(id)                                             │ │
│  │  • GetAll()                                                 │ │
│  │                                                             │ │
│  │ DiscountCalculator                                          │ │
│  │  • CalculateDiscount(price, quantity, location, date)      │ │
│  │                                                             │ │
│  │ EF Core Configurations:                                    │ │
│  │  • OrderConfiguration (table mapping)                       │ │
│  │  • OrderItemConfiguration (table mapping)                   │ │
│  │                                                             │ │
│  │ PersistenceDbContext                                        │ │
│  │  • DbSet<Order> Orders                                      │ │
│  │  • DbSet<OrderItem> OrderItems                              │ │
│  └────────────────────────────────────────────────────────────┘ │
└──────────────────────────────┬──────────────────────────────────┘
                               │
                         SQL Queries
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│                       DATABASE LAYER                            │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │ SQL Server                                                  │ │
│  │                                                             │ │
│  │ ┌──────────────────────────┐  ┌──────────────────────────┐ │ │
│  │ │ Orders Table             │  │ OrderItems Table         │ │ │
│  │ │ ┌──────────────────────┐ │  │ ┌──────────────────────┐ │ │
│  │ │ │ Id (PK)              │ │  │ │ Id (PK)              │ │ │
│  │ │ │ CustomerId (FK)      │ │  │ │ OrderId (FK)         │ │ │
│  │ │ │ FinalPrice           │ │  │ │ ProductId (FK)       │ │ │
│  │ │ │ CreatedAt            │ │  │ │ Quantity             │ │ │
│  │ │ └──────────────────────┘ │  │ │ UnitPrice            │ │ │
│  │ └──────────────────────────┘  │ └──────────────────────┘ │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

## Request/Response Flow for Creating an Order

```
CLIENT                 API CONTROLLER           APPLICATION         DOMAIN              PERSISTENCE
  │                        │                        │                 │                     │
  │  POST /api/orders       │                        │                 │                     │
  ├───────────────────────>│                        │                 │                     │
  │                        │ CreateOrderCommand      │                 │                     │
  │                        ├───────────────────────>│                 │                     │
  │                        │                        │ Handler: Create Order                │
  │                        │                        ├────────────────>│                     │
  │                        │                        │                 │ Validate Customer   │
  │                        │                        │                 ├────────────────────>│
  │                        │                        │                 │<────────────────────┤
  │                        │                        │ For each item:   │                     │
  │                        │                        ├────────────────>│ Validate Product    │
  │                        │                        │                 ├────────────────────>│
  │                        │                        │                 │<────────────────────┤
  │                        │                        │                 │ Check Stock         │
  │                        │                        │                 ├────────────────────>│
  │                        │                        │                 │<────────────────────┤
  │                        │                        │                 │ DecreaseStock       │
  │                        │                        │                 ├────────────────────>│
  │                        │                        │                 │<────────────────────┤
  │                        │                        │ Calculate Discount                   │
  │                        │                        ├────────────────>│                     │
  │                        │                        │                 │ (Complex Logic)     │
  │                        │                        │                 │<────────────────────┤
  │                        │                        │ Create Order    │                     │
  │                        │                        ├────────────────>│                     │
  │                        │                        │                 │ Save to Database    │
  │                        │                        │                 ├────────────────────>│
  │                        │                        │                 │<────────────────────┤
  │                        │<───────────────────────┤ Return Order ID │                     │
  │                        │                        │<────────────────┤                     │
  │  200 OK: {id: "..."}   │                        │                 │                     │
  │<───────────────────────┤                        │                 │                     │
  │                        │                        │                 │                     │
```

## Discount Calculation Flow

```
Input Parameters:
  • Base Price: decimal
  • Total Quantity: int
  • Customer Location: enum (US, EUROPE, ASIA)
  • Order Date: DateTime

                    ┌─────────────────────────────┐
                    │  CalculateDiscount Function │
                    └──────────────┬──────────────┘
                                   │
        ┌──────────────────────────┼──────────────────────────┐
        │                          │                          │
        ▼                          ▼                          ▼
   ┌─────────────┐    ┌──────────────────┐    ┌──────────────────┐
   │   Volume    │    │   Seasonal &     │    │   Location-based │
   │  Discount   │    │  Promotional     │    │     Pricing      │
   └──────┬──────┘    └────────┬─────────┘    └────────┬─────────┘
          │                    │                       │
      Check qty:         Check date:            Apply multiplier:
      5-9: 10%         • Black Friday: 25%     • US: 1.0x
      10-49: 20%       • Polish Holiday: 15%   • EUROPE: 1.15x
      50+: 30%         • None: 0%              • ASIA: 1.05x
          │                    │                       │
          └────────────────────┼───────────────────────┘
                               │
                    ┌──────────▼──────────┐
                    │  Select Highest     │
                    │  Discount           │
                    │  (Not Combined!)    │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │ Apply Location      │
                    │ Multiplier to       │
                    │ Base Price          │
                    │ Result = Base ×     │
                    │ Location Multiplier │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │ Apply Discount      │
                    │ Discount Amount =   │
                    │ Adjusted Price ×    │
                    │ Discount %          │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │ Final Price =       │
                    │ Adjusted Price -    │
                    │ Discount Amount     │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │   Return Final      │
                    │    Price (decimal)  │
                    └─────────────────────┘

Example:
  Base: $1000 (10 units at $100 each)
  Location: EUROPE
  Date: Black Friday

  Step 1: Applicable Discounts
    • Volume: 20% (10 units)
    • Black Friday: 25%
    • Selected: 25% (highest)

  Step 2: Location Adjustment
    • $1000 × 1.15 = $1150

  Step 3: Apply Discount
    • $1150 × 0.75 = $862.50 (25% off)

  Result: $862.50
```

## Order Creation State Transitions

```
                    ┌─────────────────────┐
                    │   POST /api/orders  │
                    │   CreateOrderCommand│
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │  Validate Customer  │
                    │  Exists?            │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
        ┌─────────>│  NO CUSTOMER?        │───────────┐
        │          │  → Exception         │           │
        │          │  → 404 Not Found     │           │
        │          └──────────────────────┘           │
        │                                             │
        │                                    ┌────────▼──────┐
        │                                    │ 400 Bad Req   │
        │                                    │ Return Error  │
        │                                    └───────────────┘
        │
        └──────────────────────────────────────┐
                                               │
                    ┌──────────────────────────▼──┐
                    │  For each OrderItem:        │
                    │  1. Validate Product Exists │
                    │  2. Check Stock >= Qty      │
                    │  3. Decrease Stock          │
                    └──────────────┬───────────────┘
                                   │
                    ┌──────────────▼──────────────┐
        ┌─────────>│  Insufficient Stock?         │──────────┐
        │          │  → InsufficientStockException│          │
        │          │  → 400 Bad Request           │          │
        │          └──────────────────────────────┘          │
        │                                                    │
        │                                           ┌────────▼──────┐
        │                                           │ 400 Bad Req   │
        │                                           │ Return Error  │
        │                                           └───────────────┘
        │
        └──────────────────────────────────────┐
                                               │
                    ┌──────────────────────────▼──┐
                    │  Calculate Final Price      │
                    │  • Get Volume Discount      │
                    │  • Get Seasonal Discount    │
                    │  • Select Highest           │
                    │  • Apply Location Adj       │
                    │  • Calculate Final Price    │
                    └──────────────┬───────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │  Create Order Entity        │
                    │  • Generate Order ID        │
                    │  • Set Customer ID          │
                    │  • Set Order Items          │
                    │  • Set Final Price          │
                    │  • Set Created Timestamp    │
                    └──────────────┬───────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │  Save to Database           │
                    │  • Insert Order             │
                    │  • Insert OrderItems        │
                    │  • Commit Transaction       │
                    └──────────────┬───────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │  200 OK                     │
                    │  {                          │
                    │    "id": "order-guid"       │
                    │  }                          │
                    └─────────────────────────────┘
```

## Class Dependencies

```
OrderController
  ├─ depends on IMediator
  └─ sends CreateOrderCommand
      ├─ dependencies:
      │   ├─ IOrderRepository
      │   ├─ IProductRepository
      │   ├─ ICustomerRepository
      │   └─ IDiscountCalculator
      └─ creates Order aggregate
          ├─ Order entity
          └─ OrderItem collection

DiscountCalculator
  ├─ calculates: decimal CalculateDiscount(...)
  ├─ uses: CustomerLocation enum
  ├─ uses: DateTime for date checks
  └─ checks for:
      ├─ Volume discounts
      ├─ Black Friday
      ├─ Polish holidays
      └─ Location-based pricing

ProductRepository
  ├─ updates Product.Stock
  └─ decreases stock by calling
      └─ Product.DecreaseStock(quantity)

Database
  ├─ Orders Table
  │   ├─ Id (PK)
  │   ├─ CustomerId (FK → Customers)
  │   ├─ FinalPrice
  │   └─ CreatedAt
  └─ OrderItems Table
      ├─ Id (PK)
      ├─ OrderId (FK → Orders)
      ├─ ProductId (FK → Products)
      ├─ Quantity
      └─ UnitPrice
```

## Data Flow for Query (GetOrders)

```
GET /api/orders
     │
     ▼
OrderController.GetOrders()
     │
     ├─ mediator.Send(new GetOrdersQuery())
     │
     ▼
GetOrdersQueryHandler.Handle()
     │
     ├─ orderRepository.GetAll()
     │
     ▼
OrderRepository.GetAll()
     │
     ├─ DbContext.Orders
     │   .Include(o => o.OrderItems)
     │   .ToListAsync()
     │
     ▼
Database Query
     │
     ├─ SELECT * FROM Orders
     │ INNER JOIN OrderItems ON Orders.Id = OrderItems.OrderId
     │
     ▼
List<Order>
     │
     ├─ MapToOrderDto() × N
     │
     ▼
List<OrderDto>
     │
     ├─ MapToOrderResponse() × N
     │
     ▼
200 OK
[
  {
    "id": "...",
    "customerId": "...",
    "items": [...],
    "finalPrice": 123.45,
    "createdAt": "..."
  },
  ...
]
```

## Polish Holiday Calendar (Annual)

```
        JANUARY           MAY           AUGUST        NOVEMBER       DECEMBER
        ┌─────┐          ┌──┐          ┌──┐          ┌──┐          ┌────┐
        │ 1st │          │1st│          │15│          │1st│          │25th│
        │ New │          │Lab│          │Ass│         │All│          │Xmas│
        │Year │          │our│          │umpt│        │Sain│         │Day │
        └─────┘          └──┘          │ion │        │ts   │        └────┘
           ▲               ▲             └──┘          └──┘              ▲
           │               │                            ▲               │
           │               │                            │               │
        6th Jan        3rd May                       11th Nov        26th Dec
        Epiphany    Constitution                 Independence      2nd Xmas
                      Day                             Day             Day

        4th Friday of November = BLACK FRIDAY (25% discount)
```

## Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                     DEPENDENCY INJECTION                            │
│                                                                     │
│  IOrderRepository ← OrderRepository                                 │
│  IProductRepository ← ProductRepository                             │
│  ICustomerRepository ← CustomerRepository                           │
│  IDiscountCalculator ← DiscountCalculator                           │
│  IUnitOfWork ← UnitOfWork                                           │
└─────────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      MEDIATR PIPELINE                               │
│                                                                     │
│  [Request] → [Validation] → [Handler] → [Response]                 │
│                              │                                      │
│                              ├─ Create Order                        │
│                              ├─ Update Stock                        │
│                              ├─ Calculate Discount                  │
│                              └─ Persist to DB                       │
│                                                                     │
│  [TransactionBehavior] wraps commands in transaction                │
└─────────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     DATABASE TRANSACTION                            │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ BEGIN TRANSACTION                                            │  │
│  │                                                              │  │
│  │  1. Insert Order                                             │  │
│  │  2. Insert OrderItems                                        │  │
│  │  3. Update Products (Stock)                                  │  │
│  │                                                              │  │
│  │ COMMIT (or ROLLBACK on error)                                │  │
│  └──────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

This architecture ensures:
- ✅ Single Responsibility Principle
- ✅ Dependency Inversion Principle
- ✅ Clean separation of concerns
- ✅ Testability through dependency injection
- ✅ Transactional consistency
- ✅ Clear data flow
- ✅ Maintainability and extensibility
