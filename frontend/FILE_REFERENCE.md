# Frontend Complete File Reference

This document provides a complete overview of all files created for the IPL Merch Store frontend.

## File Structure Summary

```
frontend/
├── Configuration Files
│   ├── package.json ........................ Dependencies and scripts
│   ├── tsconfig.json ....................... TypeScript compiler options
│   ├── tsconfig.node.json .................. TypeScript config for build files
│   ├── vite.config.ts ...................... Vite bundler configuration
│   ├── index.html .......................... HTML entry point
│   ├── .env.example ........................ Environment template
│   ├── .env.development .................... Dev environment variables
│   ├── .env.production ..................... Production environment variables
│   ├── .gitignore .......................... Git ignore patterns
│   └── README.md ........................... Full documentation
│
├── Source Files (src/)
│   ├── main.tsx ............................ React DOM entry point
│   ├── App.tsx ............................. Main app with routing
│   ├── App.css ............................. Home page styles
│   │
│   ├── components/
│   │   ├── layout/
│   │   │   ├── AppLayout.tsx .............. Main layout wrapper
│   │   │   ├── Header.tsx ................. Navigation header
│   │   │   └── Header.css ................. Header styles
│   │   │
│   │   └── shared/
│   │       ├── index.tsx .................. Shared UI components
│   │       └── shared.css ................. Component styles
│   │
│   ├── pages/
│   │   ├── ProductListPage.tsx ............ Product catalog (placeholder)
│   │   ├── ProductDetailsPage.tsx ......... Product detail (placeholder)
│   │   ├── CartPage.tsx ................... Shopping cart (placeholder)
│   │   ├── OrderHistoryPage.tsx ........... Order list (placeholder)
│   │   ├── NotFoundPage.tsx ............... 404 page
│   │   └── pages.css ...................... Page component styles
│   │
│   ├── services/
│   │   └── api.ts ......................... Axios API client
│   │
│   ├── types/
│   │   ├── common.ts ...................... Common types (ApiResponse, ApiError)
│   │   ├── product.ts ..................... Product types
│   │   ├── cart.ts ........................ Cart types
│   │   ├── order.ts ....................... Order types
│   │   └── franchise.ts ................... Franchise types
│   │
│   ├── utils/
│   │   ├── constants.ts ................... Routes, endpoints, query keys, messages
│   │   └── formatters.ts .................. Helper functions
│   │
│   ├── hooks/
│   │   └── useApi.ts ...................... Custom React Query hooks
│   │
│   └── styles/
│       ├── global.css ..................... Global styles and CSS variables
│       └── AppLayout.css .................. Layout styles
│
└── public/
    └── (empty - ready for static assets)
```

## Component Details

### Layout Components

#### AppLayout.tsx
- Main layout wrapper
- Renders: Header, main content area, footer
- Uses AppLayout.css/layout styles
- Wraps all pages

#### Header.tsx
- Sticky navigation header
- Links: Home, Products, Cart, Orders
- Active link indicators
- Logo with cricket emoji

### Shared Components

#### LoadingSpinner
```typescript
<LoadingSpinner message="Loading products..." />
```
- Animated spinner
- Optional message
- Centered display

#### EmptyState
```typescript
<EmptyState
  title="Your cart is empty"
  description="Add items to get started"
  actionText="Continue Shopping"
  onAction={() => navigate('/products')}
  icon="🛒"
/>
```
- Icon, title, description
- Optional action button
- Customizable messaging

#### ErrorBoundary
```typescript
<ErrorBoundary error={error} onRetry={() => refetch()} />
```
- Error display
- Optional retry button
- Error details shown

#### Alert
```typescript
<Alert type="success" message="Order created!" onClose={() => {}} />
```
- 4 variants: success, error, warning, info
- Slide-down animation
- Auto-closeable

#### Button
```typescript
<Button variant="primary" onClick={handleClick} loading={isLoading}>
  Checkout
</Button>
```
- 3 variants: primary, secondary, danger
- Loading state
- Disabled state support

#### Badge
```typescript
<Badge variant="success">In Stock</Badge>
```
- 4 variants: primary, success, warning, error
- Status indicators

## Type Definitions

### common.ts
- `ApiResponse<T>` - API response wrapper
- `PaginatedResponse<T>` - Paginated data
- `ApiError` class - Error handling
- `ApiErrorType` enum - Error categories

### product.ts
- `Product` - Full product info
- `ProductListItem` - List view (no description)
- `CreateProductRequest` - POST payload
- `UpdateProductRequest` - PUT payload

### cart.ts
- `CartItem` - Item in cart
- `Cart` - Complete cart
- `AddToCartRequest` - Add item payload
- `UpdateCartItemRequest` - Update quantity payload

### order.ts
- `Order` - Order summary
- `OrderDetail` - Full order info
- `OrderItem` - Item in order
- `CreateOrderResponse` - Checkout response
- `OrderStatus` enum - 4 statuses

### franchise.ts
- `Franchise` - Team/franchise info
- `FranchiseListResponse` - Franchises list

## Utility Functions

### constants.ts

**ROUTES**
```
HOME, PRODUCTS, PRODUCT_DETAILS, CART, ORDERS, NOT_FOUND
```

**API_ENDPOINTS**
```
/products
/cart/{userId}
/cart/{userId}/items
/orders/{userId}
/orders/checkout
/franchises
```

**QUERY_KEYS** (for React Query)
```
PRODUCTS, PRODUCT(id), CART(userId), ORDERS(userId), etc.
```

**HTTP_STATUS**
```
OK, CREATED, BAD_REQUEST, UNAUTHORIZED, NOT_FOUND, etc.
```

**ERROR_MESSAGES**
```
NETWORK_ERROR, SERVER_ERROR, VALIDATION_ERROR, etc.
```

**UI_MESSAGES**
```
LOADING, ADDING_TO_CART, CHECKOUT_FAILED, etc.
```

**PAGINATION**
```
DEFAULT_PAGE_SIZE: 20
DEFAULT_PAGE_NUMBER: 1
```

### formatters.ts

**Currency**
- `formatCurrency(amount, currency='INR')` → "₹999"

**Dates**
- `formatDate(date)` → "03 Apr 2024"
- `formatDateTime(date)` → "03 Apr 2024, 05:30 PM"

**Strings**
- `truncate(str, 50)` → "Lorem ipsum dolor sit amet..."
- `capitalize(str)` → "Hello"
- `formatProductName(name)` → "Proper case"
- `formatQuantity(5, 'item')` → "5 items"

**Validation**
- `isValidEmail(email)` → boolean

**Calculations**
- `getDiscountPercentage(100, 75)` → 25

**Other**
- `delay(ms)` → Promise (useful for debouncing)

## API Service

### api.ts

```typescript
const client = new ApiClient(baseURL);

// Methods:
client.get<T>(url)      // GET request
client.post<T>(url, data)   // POST request
client.put<T>(url, data)    // PUT request
client.patch<T>(url, data)  // PATCH request
client.delete<T>(url)   // DELETE request

// Usage:
const response = await apiClient.get<Product[]>('/products');
```

Features:
- Axios instance management
- Request/response interceptors
- Error transformation to `ApiError`
- TypeScript generics for type safety

## Custom Hooks

### useApi.ts

```typescript
// Generic API query
const { data, isLoading, error } = useApi<T>(
  queryKey,    // React Query key
  url,         // API endpoint
  enabled      // Conditional fetch
);

// Paginated query
const { data, isLoading, error } = usePaginatedApi<T>(
  queryKey,
  url,
  pageNumber,  // default: 1
  pageSize     // default: 20
);
```

Features:
- React Query integration
- Automatic caching (5 min stale time)
- 1 retry on failure
- Optional conditional fetching
- Type-safe data access

## Styling System

### global.css

**CSS Custom Properties**
```css
--primary-color: #1f2937
--secondary-color: #3b82f6
--success-color: #10b981
--warning-color: #f59e0b
--error-color: #ef4444
--text-primary: #1f2937
--text-secondary: #6b7280
--bg-primary: #ffffff
--border-color: #e5e7eb
--spacing-xs, -sm, -md, -lg, -xl, -2xl, -3xl
--font-xs through --font-3xl
--radius-sm through --radius-xl
--shadow-sm, -md, -lg
--transition-fast (150ms), -base (300ms), -slow (500ms)
```

**Utility Classes**
- `.container` - Max width with padding
- `.grid` - CSS Grid
- `.flex`, `.flex-center`, `.flex-between` - Flexbox
- `.w-full`, `.h-full` - Full width/height
- `.text-center`, `.text-muted` - Text utilities
- `.sr-only` - Screen reader only

### AppLayout.css

Styles for:
- `.layout` - Main container flexbox
- `.layout-header` - Sticky header
- `.layout-main` - Main content
- `.layout-content` - Scrollable content
- `.layout-footer` - Footer

### Header.css

Styles for:
- `.header` - Sticky header bar
- `.header-logo` - Logo styling
- `.header-nav` - Navigation menu
- `.header-nav-link` - Nav links with active state

### shared.css

Styles for all shared components:
- Loading spinner animation
- Empty state layout
- Error container
- Alert notifications
- Button variants
- Badge styles

### pages.css

Styles for:
- 404 page
- Page grid layouts
- Loading states

## Configuration Details

### package.json
```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.20.0",
    "@tanstack/react-query": "^5.25.0",
    "axios": "^1.6.1"
  },
  "devDependencies": {
    "@types/react": "^18.2.37",
    "@types/react-dom": "^18.2.15",
    "@vitejs/plugin-react": "^4.2.0",
    "typescript": "^5.2.2",
    "vite": "^5.0.8"
  }
}
```

### vite.config.ts
- Path aliases for all directories
- Dev server on port 5173
- Auto-open in browser
- React plugin enabled

### tsconfig.json
- Target: ES2020
- Strict mode enabled
- JSX: react-jsx
- Path mapping for aliases
- No unused variable warnings

## Usage Patterns

### Adding a New Page

1. Create component in `src/pages/NewPage.tsx`
2. Add route in `src/App.tsx`
3. Add route constant in `src/utils/constants.ts`
4. Add navigation link in `src/components/layout/Header.tsx`

### Adding Product Card Component

```typescript
// src/components/products/ProductCard.tsx
import { Product } from '@types/product';
import { formatCurrency } from '@utils/formatters';

export const ProductCard: React.FC<{ product: Product }> = ({ product }) => {
  return (
    <div className="product-card">
      <img src={product.imageUrl} alt={product.name} />
      <h3>{product.name}</h3>
      <p>{formatCurrency(product.price)}</p>
      <button>Add to Cart</button>
    </div>
  );
};
```

### Using API Data in Component

```typescript
import { useApi } from '@hooks/useApi';
import { QUERY_KEYS, API_ENDPOINTS } from '@utils/constants';
import { LoadingSpinner, ErrorBoundary } from '@components/shared';

export const ProductList = () => {
  const { data: products, isLoading, error } = useApi(
    QUERY_KEYS.PRODUCTS,
    API_ENDPOINTS.PRODUCTS
  );

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorBoundary error={error} />;

  return (
    <div className="product-grid">
      {products?.map(product => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  );
};
```

### Creating a Mutation (for POST/PUT/DELETE)

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@services/api';
import { QUERY_KEYS } from '@utils/constants';

export const useAddToCart = (userId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data) => 
      apiClient.post(`/cart/${userId}/items`, data),
    
    onSuccess: () => {
      // Invalidate so React Query refetches
      queryClient.invalidateQueries({ 
        queryKey: QUERY_KEYS.CART(userId) 
      });
    },
  });
};
```

## Development Workflow

1. **Start Dev Server**
   ```bash
   npm run dev
   ```

2. **Create Component**
   - Add `.tsx` file in appropriate folder
   - Import custom types from `@types/*`
   - Use shared components from `@components/shared`
   - Use formatters from `@utils/*`

3. **Fetch Data**
   - Use `useApi()` hook for GET requests
   - Use `useMutation()` for POST/PUT/DELETE
   - Handle loading and error states

4. **Style Component**
   - Use CSS custom properties from `global.css`
   - Create component-specific `.css` files
   - Follow BEM naming convention if needed

5. **Test in Browser**
   - Navigate to page
   - Verify styles and layout
   - Check TypeScript types

6. **Build & Deploy**
   ```bash
   npm run build
   # Upload dist/ folder to hosting
   ```

## Next Phase Implementation

Ready to implement:
- Product listing and filtering
- Shopping cart with persistence
- Checkout flow
- Order management
- User authentication
- Form validation
- Error tracking
- Analytics

All foundation is in place for rapid feature development!
