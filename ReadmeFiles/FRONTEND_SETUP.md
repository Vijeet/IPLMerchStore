# IPL Merch Store - Frontend Setup Guide

Complete frontend foundation for the IPL Merchandise Store app with Vite, React, and TypeScript.

## 📁 What's Included

✅ **Project Structure**
- Vite + React + TypeScript setup
- React Router v6 with all main routes
- React Query v5 for data fetching
- Axios HTTP client with environment-based configuration
- Comprehensive TypeScript type definitions

✅ **Components Library**
- AppLayout with sticky header
- Header with navigation
- Reusable components: LoadingSpinner, EmptyState, ErrorBoundary, Alert, Button, Badge
- Placeholder pages: ProductList, ProductDetails, Cart, OrderHistory

✅ **Infrastructure**
- Global CSS with CSS variables for theming
- Path aliases for clean imports (@components, @services, etc.)
- API client with error handling
- Custom React Query hooks (useApi, useFetch, usePaginatedApi)
- Utility functions: formatCurrency, formatDate, truncate, capitalize, etc.
- Constants for routes, API endpoints, query keys, and messages

✅ **Development Ready**
- ESLint configuration ready to add
- TypeScript strict mode enabled
- Hot module replacement (HMR) with Vite
- Mobile-first responsive design
- No external UI libraries (CSS only)

## 🚀 Quick Start

### 1. Install Dependencies

```bash
cd frontend
npm install
```

### 2. Configure Environment

The `.env.development` file is already set up:
```env
VITE_API_BASE_URL=http://localhost:5000/api
```

Update this if your backend runs on a different port.

### 3. Start Development Server

```bash
npm run dev
```

The app will open at `http://localhost:5173`

**Hot reload enabled**: Changes to source files automatically refresh the browser.

### 4. Build for Production

```bash
npm run build
```

Minified build output goes to `dist/` directory.

## 📂 Project Structure

```
frontend/
├── src/
│   ├── components/layout/       # AppLayout, Header components
│   ├── components/shared/       # Reusable UI components
│   ├── pages/                   # Page components
│   ├── services/api.ts          # Axios API client
│   ├── types/                   # TypeScript type definitions
│   ├── utils/                   # Formatters and constants
│   ├── hooks/useApi.ts          # Custom React Query hooks
│   ├── styles/                  # Global styles
│   ├── App.tsx                  # Main app with routing
│   └── main.tsx                 # Entry point
├── public/                      # Static files
├── index.html                   # HTML template
├── package.json                 # Dependencies
├── tsconfig.json               # TypeScript config
├── vite.config.ts              # Vite config
└── README.md                   # Detailed documentation
```

## 🔗 API Integration

The API client is pre-configured in `src/services/api.ts`:

```typescript
import { apiClient } from '@services/api';

// GET request
const response = await apiClient.get('/products');

// POST request
const response = await apiClient.post('/orders/checkout', { userId });

// Error handling
try {
  // ...
} catch (error) {
  if (error instanceof ApiError) {
    console.log(error.type, error.statusCode, error.details);
  }
}
```

### React Query Integration

Use the custom hooks for automatic caching and refetching:

```typescript
import { useApi } from '@hooks/useApi';
import { QUERY_KEYS, API_ENDPOINTS } from '@utils/constants';

export const ProductList = () => {
  const { data, isLoading, error } = useApi(
    QUERY_KEYS.PRODUCTS,
    API_ENDPOINTS.PRODUCTS
  );

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorBoundary error={error} />;
  
  return <div>{/* Render products */}</div>;
};
```

## 🎨 Styling & Theming

Global styles use CSS custom properties:

**Edit `src/styles/global.css` to customize:**
- Colors: `--primary-color`, `--secondary-color`, `--error-color`, etc.
- Spacing: `--spacing-xs` through `--spacing-3xl`
- Typography: `--font-xs` through `--font-3xl`
- Shadows, border radius, transitions, z-index layers

Example:
```css
:root {
  --primary-color: #1f2937;
  --secondary-color: #3b82f6;
  /* ... */
}
```

## 📦 Included Components

### Layout
- **AppLayout** - Main layout wrapper with header and footer
- **Header** - Navigation with active link indicators

### Shared UI
- **LoadingSpinner** - Animated loader with optional message
- **EmptyState** - Placeholder for empty content
- **ErrorBoundary** - Error display with retry button
- **Alert** - Notifications (success, error, warning, info)
- **Button** - Styled button (primary, secondary, danger)
- **Badge** - Status badges

### Pages
- **ProductListPage** - Placeholder for product catalog
- **ProductDetailsPage** - Placeholder for product detail view
- **CartPage** - Placeholder for shopping cart
- **OrderHistoryPage** - Placeholder for order history
- **NotFoundPage** - 404 page

## 🛣️ Routes

| Route | Page | Status |
|-------|------|--------|
| `/` | Home | ✅ Ready |
| `/products` | Product List | 📋 Placeholder |
| `/products/:id` | Product Details | 📋 Placeholder |
| `/cart` | Shopping Cart | 📋 Placeholder |
| `/orders` | Order History | 📋 Placeholder |
| `/404` | Not Found | ✅ Ready |

## 🔑 Path Aliases

Use these for clean imports:

```typescript
// Components
import { AppLayout } from '@components/layout/AppLayout';
import { LoadingSpinner } from '@components/shared';

// Services
import { apiClient } from '@services/api';

// Types
import type { Product } from '@types/product';

// Utils
import { formatCurrency } from '@utils/formatters';
import { ROUTES, QUERY_KEYS } from '@utils/constants';

// Hooks
import { useApi } from '@hooks/useApi';

// Styles
import '@styles/global.css';
```

## 📋 Implementation Checklist

### Phase 1: Product Management
- [ ] Implement ProductListPage with product grid
- [ ] Create ProductCard component
- [ ] Fetch products from API using useApi
- [ ] Add product filtering/search
- [ ] Implement ProductDetailsPage with full details
- [ ] Add image gallery component

### Phase 2: Shopping Cart
- [ ] Implement cart state management (Context or Zustand)
- [ ] Create CartItem component
- [ ] Implement add/remove from cart
- [ ] Show cart summary with total price
- [ ] Persist cart to localStorage

### Phase 3: Checkout & Orders
- [ ] Implement checkout flow
- [ ] Create checkout form component
- [ ] Integrate payment processing
- [ ] Implement OrderHistoryPage
- [ ] Show order details

### Phase 4: Authentication (Optional)
- [ ] Create login/signup pages
- [ ] Implement auth context
- [ ] Add protected routes
- [ ] Session management

### Phase 5: Polish
- [ ] Add loading states to all operations
- [ ] Implement error handling
- [ ] Add success notifications
- [ ] Mobile optimization
- [ ] Accessibility improvements

## 📝 Type System

Comprehensive types are defined in `src/types/`:

```typescript
// Product types
type Product = { id, name, price, imageUrl, ... }
type ProductListItem = Omit<Product, 'description'>

// Cart types
type CartItem = { productId, quantity, price, ... }
type Cart = { userId, items, totalPrice, ... }

// Order types
type Order = { id, userId, items, status, ... }
enum OrderStatus { Pending, Processing, Completed, Cancelled }

// Common
type ApiResponse<T> = { success, data, message, errors }
class ApiError extends Error { type, statusCode, details }
```

## 🎯 Usage Examples

### Fetching Data
```typescript
const { data: products, isLoading, error } = useApi(
  QUERY_KEYS.PRODUCTS,
  API_ENDPOINTS.PRODUCTS
);
```

### Formatting Values
```typescript
import { formatCurrency, formatDate } from '@utils/formatters';

const price = formatCurrency(999); // ₹999
const date = formatDate('2024-04-03'); // 03 Apr 2024
```

### Navigation
```typescript
import { useNavigate } from 'react-router-dom';
import { ROUTES } from '@utils/constants';

const navigate = useNavigate();
navigate(ROUTES.PRODUCTS);
```

### API Mutations (for POST/PUT/DELETE)
```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@services/api';

const mutation = useMutation({
  mutationFn: (data) => apiClient.post('/orders/checkout', data),
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ORDERS });
  },
});
```

## 🔧 Configuration Files

### vite.config.ts
- Path aliases configured
- Development server on port 5173
- Auto-open in browser

### tsconfig.json
- Strict mode enabled
- Path aliases configured
- JSX support with React 18

### package.json
- All dependencies listed with versions
- Scripts for dev, build, preview, lint

## 🌐 Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers

## 📞 Support

Refer to:
- `frontend/README.md` - Detailed documentation
- `src/services/api.ts` - API client implementation
- `src/utils/constants.ts` - Routes and endpoints
- `src/types/` - Type definitions

## 🎓 Next Steps

1. **Run development server**: `npm run dev`
2. **Explore the pages**: Navigate through routes
3. **Start implementing pages**: Begin with ProductListPage
4. **Connect to backend**: Update API endpoints as needed
5. **Add more components**: Build custom UI components as needed

Happy coding! 🚀
