# Cart Module Implementation

## Overview

The Cart module provides a complete shopping cart management API for the IPL Merch Store. It allows users to add, update, and remove items from their cart with real-time inventory validation and price capture.

**Key Features:**
- One active cart per user (no authentication required for demo)
- Automatic cart creation on first item addition
- Quantity increment on adding existing products
- Real-time inventory validation
- Price snapshot capture at time of addition
- Empty state handling for frontend

---

## Architecture

### Domain Layer (IplMerchStore.Domain)
- **Cart** Entity: User shopping cart container
- **CartItem** Entity: Individual items in the cart with quantity and price snapshot

### Application Layer (IplMerchStore.Application)
- **DTOs:**
  - `AddCartItemRequest`: Add item request (ProductId, Quantity)
  - `UpdateCartItemRequest`: Update item quantity
  - `CartItemResponse`: Item response with product snapshot
  - `CartResponse`: Complete cart with items, totals, and metadata

- **Interface:** `ICartService` - Service contract for cart operations

### Infrastructure Layer (IplMerchStore.Infrastructure)
- **CartService**: Full business logic implementation
  - Lazy cart creation
  - Inventory validation
  - Price capture and updates
  - Transactional operations

### API Layer (IplMerchStore.Api)
- **CartController**: RESTful endpoints with thin validation

---

## API Endpoints

### 1. GET /api/cart/{userId}
**Retrieve user's cart**

**Path Parameters:**
- `userId` (string) - User identifier

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Cart retrieved successfully",
  "data": {
    "id": 1,
    "userId": "user123",
    "items": [
      {
        "id": 1,
        "productId": 1,
        "productName": "MI Jersey",
        "productImageUrl": "https://example.com/mi-jersey.jpg",
        "productSku": "SKU001",
        "unitPrice": 1299.00,
        "quantity": 2,
        "subtotal": 2598.00,
        "currentInventory": 50,
        "isProductActive": true
      }
    ],
    "itemCount": 1,
    "totalQuantity": 2,
    "totalAmount": 2598.00,
    "currency": "INR",
    "isEmpty": false
  }
}
```

**Response 404 Not Found:**
```json
{
  "success": false,
  "message": "No active cart found for user",
  "data": null
}
```

---

### 2. POST /api/cart/{userId}/items
**Add item to cart (or increment quantity if exists)**

**Path Parameters:**
- `userId` (string) - User identifier

**Request Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

**Response 200 OK (Item Added):**
```json
{
  "success": true,
  "message": "Item added to cart successfully",
  "data": {
    "id": 1,
    "userId": "user123",
    "items": [
      {
        "id": 1,
        "productId": 1,
        "productName": "MI Jersey",
        "productImageUrl": "https://example.com/mi-jersey.jpg",
        "productSku": "SKU001",
        "unitPrice": 1299.00,
        "quantity": 2,
        "subtotal": 2598.00,
        "currentInventory": 50,
        "isProductActive": true
      }
    ],
    "itemCount": 1,
    "totalQuantity": 2,
    "totalAmount": 2598.00,
    "currency": "INR",
    "isEmpty": false
  }
}
```

**Response 200 OK (Quantity Incremented):**
```json
{
  "success": true,
  "message": "Item added to cart successfully",
  "data": {
    "id": 1,
    "userId": "user123",
    "items": [
      {
        "id": 1,
        "productId": 1,
        "productName": "MI Jersey",
        "productImageUrl": "https://example.com/mi-jersey.jpg",
        "productSku": "SKU001",
        "unitPrice": 1299.00,
        "quantity": 5,
        "subtotal": 6495.00,
        "currentInventory": 50,
        "isProductActive": true
      }
    ],
    "itemCount": 1,
    "totalQuantity": 5,
    "totalAmount": 6495.00,
    "currency": "INR",
    "isEmpty": false
  }
}
```

**Response 400 Bad Request (Invalid Quantity):**
```json
{
  "success": false,
  "message": "Quantity must be greater than 0",
  "errors": null
}
```

**Response 400 Bad Request (Inventory Exceeded):**
```json
{
  "success": false,
  "message": "Cannot add 100 units. Only 50 units available in stock",
  "errors": null
}
```

**Response 404 Not Found (Product Not Found):**
```json
{
  "success": false,
  "message": "Product with ID 999 not found",
  "errors": null
}
```

**Response 400 Bad Request (Inactive Product):**
```json
{
  "success": false,
  "message": "Product 'MI Jersey' is no longer available",
  "errors": null
}
```

---

### 3. PUT /api/cart/{userId}/items/{productId}
**Update cart item quantity**

**Path Parameters:**
- `userId` (string) - User identifier
- `productId` (integer) - Product ID to update

**Request Body:**
```json
{
  "quantity": 5
}
```

**Response 200 OK (Quantity Updated):**
```json
{
  "success": true,
  "message": "Cart item updated successfully",
  "data": {
    "id": 1,
    "userId": "user123",
    "items": [
      {
        "id": 1,
        "productId": 1,
        "productName": "MI Jersey",
        "productImageUrl": "https://example.com/mi-jersey.jpg",
        "productSku": "SKU001",
        "unitPrice": 1299.00,
        "quantity": 5,
        "subtotal": 6495.00,
        "currentInventory": 50,
        "isProductActive": true
      }
    ],
    "itemCount": 1,
    "totalQuantity": 5,
    "totalAmount": 6495.00,
    "currency": "INR",
    "isEmpty": false
  }
}
```

**Response 200 OK (Item Removed - Quantity = 0):**
```json
{
  "success": true,
  "message": "Item removed from cart successfully",
  "data": {
    "id": 1,
    "userId": "user123",
    "items": [],
    "itemCount": 0,
    "totalQuantity": 0,
    "totalAmount": 0.00,
    "currency": "INR",
    "isEmpty": true
  }
}
```

**Response 400 Bad Request (Inventory Exceeded):**
```json
{
  "success": false,
  "message": "Cannot update to 100 units. Only 50 units available in stock",
  "errors": null
}
```

**Response 404 Not Found (Product Not in Cart):**
```json
{
  "success": false,
  "message": "Product with ID 999 not found in cart",
  "errors": null
}
```

---

### 4. DELETE /api/cart/{userId}/items/{productId}
**Remove item from cart**

**Path Parameters:**
- `userId` (string) - User identifier
- `productId` (integer) - Product ID to remove

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Item removed from cart successfully",
  "data": {
    "id": 1,
    "userId": "user123",
    "items": [],
    "itemCount": 0,
    "totalQuantity": 0,
    "totalAmount": 0.00,
    "currency": "INR",
    "isEmpty": true
  }
}
```

**Response 404 Not Found:**
```json
{
  "success": false,
  "message": "Product with ID 999 not found in cart",
  "errors": null
}
```

---

### 5. DELETE /api/cart/{userId}
**Clear entire cart**

**Path Parameters:**
- `userId` (string) - User identifier

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Cart cleared successfully"
}
```

---

## Business Rules

### 1. Product Validation
✅ Product must exist in the system
✅ Product must be active (IsActive = true)
✅ Inactive products cannot be added to cart

### 2. Inventory Validation
✅ Product must have available inventory (InventoryCount > 0) for new items
✅ Total quantity cannot exceed available inventory
✅ When adding to existing item, new total is validated: `existing_qty + new_qty <= inventory`
✅ Meaningful error messages include available quantities

### 3. Quantity Rules
✅ Adding: Quantity must be > 0
✅ Updating: Quantity must be >= 0
✅ Setting quantity to 0 removes the item from cart
✅ Adding existing product increments quantity instead of replacing

### 4. Price Capture
✅ UnitPrice is captured from current product price at time of addition
✅ Price is updated when quantity is modified (to current product price)
✅ Subtotal = UnitPrice × Quantity (calculated on response)
✅ TotalAmount = Sum of all subtotals

### 5. Cart Management
✅ One active cart per userId
✅ Cart is created on first item addition (lazy creation)
✅ Empty cart returns cleanly with IsEmpty = true
✅ Different users have independent carts

---

## Testing

### Unit Tests (CartServiceTests.cs)
**Coverage:** 30+ test cases covering:
- Cart retrieval for existing and non-existing users
- Adding items (new and existing)
- Quantity increments on duplicates
- Inventory validation
- Price capture and updates
- Item removal
- Cart clearing
- Total calculations
- Product snapshots
- Edge cases

**Run Command:**
```bash
dotnet test backend/tests/IplMerchStore.UnitTests/CartServiceTests.cs
```

### Integration Tests (CartControllerTests.cs)
**Coverage:** 25+ end-to-end tests covering:
- All 5 API endpoints
- HTTP status codes
- Request/response validation
- Multi-user isolation
- Error scenarios
- Business rule enforcement
- Total calculations
- Product data inclusion

**Run Command:**
```bash
dotnet test backend/tests/IplMerchStore.IntegrationTests/CartControllerTests.cs
```

---

## Sample cURL Requests

### Add Item to Cart
```bash
curl -X POST http://localhost:5000/api/cart/user123/items \
  -H "Content-Type: application/json" \
  -d '{
    "productId": 1,
    "quantity": 2
  }'
```

### Get Cart
```bash
curl http://localhost:5000/api/cart/user123
```

### Update Item Quantity
```bash
curl -X PUT http://localhost:5000/api/cart/user123/items/1 \
  -H "Content-Type: application/json" \
  -d '{
    "quantity": 5
  }'
```

### Remove Item from Cart
```bash
curl -X DELETE http://localhost:5000/api/cart/user123/items/1
```

### Clear Cart
```bash
curl -X DELETE http://localhost:5000/api/cart/user123
```

---

## Implementation Summary

### Files Created/Modified

**New Files:**
- `backend/src/IplMerchStore.Application/DTOs/AddCartItemRequest.cs` - Add item request DTO
- `backend/src/IplMerchStore.Application/DTOs/UpdateCartItemRequest.cs` - Update quantity request DTO
- `backend/src/IplMerchStore.Application/DTOs/CartItemResponse.cs` - Cart item response with snapshot
- `backend/src/IplMerchStore.Application/DTOs/CartResponse.cs` - Complete cart response DTO
- `backend/src/IplMerchStore.Infrastructure/Services/CartService.cs` - Service implementation (400+ lines)
- `backend/src/IplMerchStore.Api/Controllers/CartController.cs` - Controller with 5 endpoints
- `backend/tests/IplMerchStore.UnitTests/CartServiceTests.cs` - 30+ unit tests
- `backend/tests/IplMerchStore.IntegrationTests/CartControllerTests.cs` - 25+ integration tests

**Modified Files:**
- `backend/src/IplMerchStore.Application/Interfaces/ICartService.cs` - Updated interface with detailed signatures and documentation
- `backend/src/IplMerchStore.Api/Program.cs` - Added CartService DI registration

### Lines of Code
- **DTOs:** ~150 lines
- **ICartService Interface:** ~60 lines (with comprehensive documentation)
- **CartService Implementation:** ~400 lines
- **CartController:** ~200 lines
- **Unit Tests:** ~450 lines
- **Integration Tests:** ~400 lines
- **Total:** ~1,660 lines of well-documented, tested code

---

## Key Design Decisions

### 1. Lazy Cart Creation
Cart is created on first item addition rather than upfront. This reduces database operations and supports zero-cart state.

### 2. UnitPrice Snapshot
UnitPrice is captured and stored at time of addition, allowing historical pricing if product prices change. It's also updated on quantity changes to get current pricing.

### 3. Request-based Parameters
The service takes request objects (`AddCartItemRequest`, `UpdateCartItemRequest`) rather than individual parameters for better maintainability and future extensibility.

### 4. Product Snapshot
CartItemResponse includes product name, SKU, image URL, inventory, and active status - providing frontend with rich data for rendering without additional lookups.

### 5. Empty State Handling
Cart returns cleanly with `IsEmpty = true` and empty Items collection, rather than null or error states, making frontend handling simpler.

### 6. Transactional at Service Level
Each service method performs atomic operations (add, update, or remove) and reloads the cart to ensure response accuracy.

---

## Future Enhancements

- [ ] Cart persistence across sessions
- [ ] Abandoned cart recovery
- [ ] Cart sharing/wish list features
- [ ] Quantity discounts
- [ ] Promotional codes
- [ ] Cart merging for logged-in users
- [ ] Analytics on cart abandonment
- [ ] Expiring cart items based on inventory

---

## Developers

**Module Lead:** Senior Backend Developer
**Implemented:** Cart Module v1.0
**Date:** 2026-04-03
**Status:** Ready for Integration Testing

