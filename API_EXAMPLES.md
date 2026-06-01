# Order API - Endpoint Examples and Usage

## Overview
This document provides detailed examples of how to use the Order API endpoints, including request/response formats and various scenarios.

## Base URL
```
https://localhost:7200/api/orders
```

## Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders` | Create a new order |
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders/{orderId}` | Get a specific order |

---

## 1. POST /api/orders - Create Order

### Description
Creates a new order with the specified items. Validates customer exists, checks product stock, calculates discounts, and decreases inventory.

### Request

**URL**: `POST /api/orders`

**Headers**:
```
Content-Type: application/json
```

**Body**:
```json
{
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "items": [
    {
      "productId": "660e8400-e29b-41d4-a716-446655440001",
      "quantity": 5
    },
    {
      "productId": "660e8400-e29b-41d4-a716-446655440002",
      "quantity": 3
    }
  ]
}
```

### Response

**Status Code**: `200 OK`

**Body**:
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440003"
}
```

### Example 1: Basic Order (Single Product, US Customer)

**Scenario**: Customer from US orders 5 units of a product at $100 each.
- Volume discount: 10% (5+ units)
- Location adjustment: None (US = 1.0x)
- Final price: $500 × 0.9 = $450

**Request**:
```bash
curl -X POST https://localhost:7200/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440000",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 5
      }
    ]
  }'
```

**Response**:
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440003"
}
```

### Example 2: Multi-Product Order (Europe Customer with VAT)

**Scenario**: European customer orders 2 products totaling 8 units.
- Product 1: $100 × 2 units = $200
- Product 2: $80 × 6 units = $480
- Subtotal: $680
- No volume discount (less than 10 units total)
- Location adjustment: 15% VAT
- Final price: $680 × 1.15 = $782

**Request**:
```bash
curl -X POST https://localhost:7200/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440001",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 2
      },
      {
        "productId": "660e8400-e29b-41d4-a716-446655440002",
        "quantity": 6
      }
    ]
  }'
```

### Example 3: Bulk Order (Asia Customer, Heavy Discount)

**Scenario**: Asian customer orders 50 units for heavy discount.
- Base price: $100 × 50 = $5000
- Volume discount: 30% (50+ units)
- Location adjustment: 5% logistics cost
- Calculation:
  - With location: $5000 × 1.05 = $5250
  - With discount: $5250 × 0.7 = $3675

**Request**:
```bash
curl -X POST https://localhost:7200/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440002",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 50
      }
    ]
  }'
```

### Example 4: Black Friday Order

**Scenario**: Order placed on Black Friday (4th Friday of November).
- Base price: $1000 (10 units × $100)
- Volume discount: 20% (10 units)
- Seasonal discount: 25% (Black Friday)
- Applied discount: 25% (highest of 20% or 25%)
- Location: US (no adjustment)
- Final price: $1000 × 0.75 = $750

**Request** (assuming current date is Black Friday):
```bash
curl -X POST https://localhost:7200/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440000",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 10
      }
    ]
  }'
```

### Example 5: Complex Multi-Item Order

**Scenario**: European customer on Polish holiday orders multiple items.
- Item 1: $150 × 5 = $750
- Item 2: $200 × 3 = $600
- Item 3: $100 × 2 = $200
- Subtotal: $1550
- Total items: 10 units
- Volume discount: 20% (10 units)
- Polish holiday discount: 15%
- Applied discount: 20% (highest)
- Location: Europe (15% VAT)
- Calculation:
  - With location: $1550 × 1.15 = $1782.50
  - With discount: $1782.50 × 0.8 = $1426

**Request** (assuming current date is a Polish holiday):
```bash
curl -X POST https://localhost:7200/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440001",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 5
      },
      {
        "productId": "660e8400-e29b-41d4-a716-446655440002",
        "quantity": 3
      },
      {
        "productId": "660e8400-e29b-41d4-a716-446655440003",
        "quantity": 2
      }
    ]
  }'
```

### Error Cases

#### 1. Non-existent Customer
**Status Code**: `404 Not Found`

**Response**:
```json
{
  "error": "Customer with id 550e8400-e29b-41d4-a716-446655440999 doesn't exist"
}
```

#### 2. Insufficient Stock
**Status Code**: `400 Bad Request`

**Response**:
```json
{
  "error": "Insufficient stock for product 'Laptop'. Requested: 10, Available: 3"
}
```

#### 3. Non-existent Product
**Status Code**: `404 Not Found`

**Response**:
```json
{
  "error": "Product with id 660e8400-e29b-41d4-a716-446655440999 doesn't exist"
}
```

#### 4. Empty Items List
**Status Code**: `400 Bad Request`

**Response**:
```json
{
  "error": "Order must contain at least one item"
}
```

#### 5. Invalid Quantity (Negative)
**Status Code**: `400 Bad Request`

**Response**:
```json
{
  "error": "Validation failed: Quantity must be greater than 0"
}
```

---

## 2. GET /api/orders - Get All Orders

### Description
Retrieves a list of all orders in the system with their details including items and final price.

### Request

**URL**: `GET /api/orders`

**Headers**:
```
Accept: application/json
```

### Response

**Status Code**: `200 OK`

**Body**:
```json
[
  {
    "id": "770e8400-e29b-41d4-a716-446655440001",
    "customerId": "550e8400-e29b-41d4-a716-446655440000",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 5,
        "unitPrice": 100.00
      }
    ],
    "finalPrice": 450.00,
    "createdAt": "2024-01-15T10:30:00Z"
  },
  {
    "id": "770e8400-e29b-41d4-a716-446655440002",
    "customerId": "550e8400-e29b-41d4-a716-446655440001",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 2,
        "unitPrice": 100.00
      },
      {
        "productId": "660e8400-e29b-41d4-a716-446655440002",
        "quantity": 6,
        "unitPrice": 80.00
      }
    ],
    "finalPrice": 782.00,
    "createdAt": "2024-01-16T14:45:00Z"
  }
]
```

### Example Request

**cURL**:
```bash
curl -X GET https://localhost:7200/api/orders \
  -H "Accept: application/json"
```

**PowerShell**:
```powershell
$response = Invoke-RestMethod -Uri "https://localhost:7200/api/orders" -Method Get
$response | ConvertTo-Json -Depth 10
```

**C# HttpClient**:
```csharp
using (var client = new HttpClient())
{
    var response = await client.GetAsync("https://localhost:7200/api/orders");
    var content = await response.Content.ReadAsStringAsync();
    var orders = JsonConvert.DeserializeObject<List<OrderResponse>>(content);

    foreach (var order in orders)
    {
        Console.WriteLine($"Order {order.Id}: {order.FinalPrice:C}");
    }
}
```

---

## 3. GET /api/orders/{orderId} - Get Specific Order

### Description
Retrieves details of a specific order by its ID, including all items and calculated final price.

### Request

**URL**: `GET /api/orders/{orderId}`

**Parameters**:
- `orderId` (path parameter, required): UUID of the order

**Headers**:
```
Accept: application/json
```

### Response

**Status Code**: `200 OK` (if order exists)

**Body**:
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440001",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "items": [
    {
      "productId": "660e8400-e29b-41d4-a716-446655440001",
      "quantity": 5,
      "unitPrice": 100.00
    }
  ],
  "finalPrice": 450.00,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### Example Requests

**Valid Order**:
```bash
curl -X GET https://localhost:7200/api/orders/770e8400-e29b-41d4-a716-446655440001 \
  -H "Accept: application/json"
```

**Non-existent Order**:
```bash
curl -X GET https://localhost:7200/api/orders/999e8400-e29b-41d4-a716-446655440999 \
  -H "Accept: application/json"
```

**Response** (404 Not Found):
```json
{
  "error": "Order with id 999e8400-e29b-41d4-a716-446655440999 doesn't exist"
}
```

### Example C# Usage

```csharp
using (var client = new HttpClient())
{
    var orderId = Guid.Parse("770e8400-e29b-41d4-a716-446655440001");

    try
    {
        var response = await client.GetAsync($"https://localhost:7200/api/orders/{orderId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<OrderResponse>(content);

            Console.WriteLine($"Order {order.Id}:");
            Console.WriteLine($"  Customer: {order.CustomerId}");
            Console.WriteLine($"  Items: {order.Items.Count}");
            Console.WriteLine($"  Final Price: {order.FinalPrice:C}");
            Console.WriteLine($"  Created: {order.CreatedAt:g}");
        }
        else
        {
            Console.WriteLine($"Order not found: {response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
```

---

## Discount Calculation Examples

### Example 1: Small Order (US Customer)
```
Scenario:
- Customer Location: US
- Products: 1 unit × $100
- No seasonal discount

Calculation:
- Base Price: $100
- Volume Discount: None (less than 5)
- Location Adjustment: 1.0x (US)
- Final Price: $100

Request:
{
  "customerId": "...",
  "items": [{"productId": "...", "quantity": 1}]
}

Response: {"id": "...", "finalPrice": 100.00}
```

### Example 2: European Order with VAT
```
Scenario:
- Customer Location: Europe
- Products: 2 units × $100
- No seasonal discount

Calculation:
- Base Price: $200
- Volume Discount: None (less than 5)
- Location Adjustment: 1.15x (Europe VAT)
- Adjusted Price: $200 × 1.15 = $230
- Final Price: $230

Request:
{
  "customerId": "...",
  "items": [{"productId": "...", "quantity": 2}]
}

Response: {"id": "...", "finalPrice": 230.00}
```

### Example 3: Bulk Order with Maximum Discount
```
Scenario:
- Customer Location: Asia
- Products: 50 units × $100
- Black Friday (25% discount > 30% volume discount)

Calculation:
- Base Price: $5000
- Applicable Discounts:
  * Volume (50+ units): 30%
  * Black Friday: 25%
  * Selected: 30% (highest)
- Location Adjustment: 1.05x (Asia logistics)
- Adjusted Price: $5000 × 1.05 = $5250
- Final Price: $5250 × 0.7 = $3675

Request:
{
  "customerId": "...",
  "items": [{"productId": "...", "quantity": 50}]
}

Response: {"id": "...", "finalPrice": 3675.00}
```

### Example 4: Mixed Volume and Holiday Discount
```
Scenario:
- Customer Location: Europe
- Products: 10 units × $100
- New Year's Day (15% holiday discount)

Calculation:
- Base Price: $1000
- Applicable Discounts:
  * Volume (10 units): 20%
  * Holiday: 15%
  * Selected: 20% (highest)
- Location Adjustment: 1.15x (Europe VAT)
- Adjusted Price: $1000 × 1.15 = $1150
- Final Price: $1150 × 0.8 = $920

Request:
{
  "customerId": "...",
  "items": [{"productId": "...", "quantity": 10}]
}

Response: {"id": "...", "finalPrice": 920.00}
```

---

## Complete Order Workflow Example

### Step 1: Create a Product
```bash
curl -X POST https://localhost:7200/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "description": "High-performance laptop",
    "price": 1000,
    "stock": 50
  }'
```
Response: `{"productId": "660e8400-e29b-41d4-a716-446655440001"}`

### Step 2: Create a Customer
```bash
curl -X POST https://localhost:7200/api/customers \
  -H "Content-Type: application/json" \
  -d '{"location": "EUROPE"}'
```
Response: `{"customerId": "550e8400-e29b-41d4-a716-446655440001"}`

### Step 3: Create an Order
```bash
curl -X POST https://localhost:7200/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440001",
    "items": [
      {
        "productId": "660e8400-e29b-41d4-a716-446655440001",
        "quantity": 5
      }
    ]
  }'
```
Response: `{"id": "770e8400-e29b-41d4-a716-446655440001"}`

### Step 4: Verify Order
```bash
curl -X GET https://localhost:7200/api/orders/770e8400-e29b-41d4-a716-446655440001 \
  -H "Accept: application/json"
```

Response:
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440001",
  "customerId": "550e8400-e29b-41d4-a716-446655440001",
  "items": [
    {
      "productId": "660e8400-e29b-41d4-a716-446655440001",
      "quantity": 5,
      "unitPrice": 1000.00
    }
  ],
  "finalPrice": 5750.00,
  "createdAt": "2024-01-20T09:15:00Z"
}
```

### Step 5: Verify Product Stock Decreased
```bash
curl -X GET https://localhost:7200/api/products \
  -H "Accept: application/json"
```

Response shows product stock decreased from 50 to 45.

---

## Polish Bank Holidays for Discount Calculation

The system recognizes these Polish bank holidays for the 15% seasonal discount:

1. **January 1** - New Year's Day
2. **January 6** - Epiphany
3. **May 1** - Labour Day
4. **May 3** - Constitution Day
5. **August 15** - Assumption of Mary
6. **November 1** - All Saints' Day
7. **November 11** - Independence Day
8. **December 25** - Christmas Day
9. **December 26** - Second Day of Christmas

**Note**: Black Friday (4th Friday of November) supersedes regular seasonal discount if it applies to the same date.

---

## Response Time Expectations

- **Create Order**: 200-500ms (includes validation and discount calculation)
- **Get All Orders**: 100-300ms (depending on order count)
- **Get Single Order**: 50-150ms

---

## Rate Limiting

Currently, no rate limiting is implemented. In production, consider:
- Implement rate limiting by IP
- Implement rate limiting by customer ID
- Use sliding window or token bucket algorithm
- Return 429 Too Many Requests when limit exceeded

---

## Error Handling Summary

| Error | Status | Description |
|-------|--------|-------------|
| Invalid Customer ID | 404 | Customer not found in database |
| Invalid Product ID | 404 | Product not found in database |
| Insufficient Stock | 400 | Requested quantity exceeds available stock |
| Empty Items | 400 | Order must contain at least one item |
| Invalid Order ID | 404 | Order not found when retrieving |
| Negative Quantity | 400 | Quantity must be positive |
| Invalid Price | 400 | Price calculation resulted in invalid value |
