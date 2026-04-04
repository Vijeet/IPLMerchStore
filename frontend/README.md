# IPL Merchandise Store - Frontend

A modern, scalable React + TypeScript frontend for the IPL Merchandise Store with complete project foundation.

## Tech Stack

- **Framework**: React 18.2
- **Language**: TypeScript 5.2
- **Build Tool**: Vite 5.0
- **Routing**: React Router v6
- **State Management & Data Fetching**: React Query (TanStack Query) v5
- **HTTP Client**: Axios 1.6
- **Styling**: CSS3 with CSS Custom Properties (no UI framework)

## Project Structure

```
frontend/
├── src/
│   ├── components/
│   │   ├── layout/              # Layout components
│   │   │   ├── AppLayout.tsx    # Main layout wrapper
│   │   │   └── Header.tsx       # Navigation header
│   │   └── shared/              # Reusable components
│   │       ├── index.tsx        # LoadingSpinner, EmptyState, ErrorBoundary, Alert, Button, Badge
│   │       └── shared.css       # Component styles
│   ├── pages/                   # Page components
│   │   ├── ProductListPage.tsx  # Product catalog (placeholder)
│   │   ├── ProductDetailsPage.tsx
│   │   ├── CartPage.tsx
│   │   ├── OrderHistoryPage.tsx
│   │   ├── NotFoundPage.tsx
│   │   └── pages.css
│   ├── services/                # API services
│   │   └── api.ts              # Axios API client with base configuration
│   ├── types/                   # TypeScript type definitions
│   │   ├── common.ts           # Common types (ApiResponse, ApiError, etc.)
│   │   ├── product.ts          # Product types
│   │   ├── cart.ts             # Cart types
│   │   ├── order.ts            # Order types
│   │   └── franchise.ts        # Franchise types
│   ├── utils/
│   │   ├── constants.ts        # Routes, API endpoints, query keys, messages
│   │   └── formatters.ts       # Helper functions (formatCurrency, formatDate, etc.)
│   ├── hooks/
│   │   └── useApi.ts           # Custom hooks for API queries
│   ├── styles/
│   │   ├── global.css          # Global styles and CSS variables
│   │   └── AppLayout.css       # Layout styles
│   ├── App.tsx                 # Main App component with routing
│   ├── App.css                 # Home page styles
│   └── main.tsx                # React DOM entry point
├── public/                      # Static assets
├── index.html                   # HTML entry point
├── package.json                 # Dependencies and scripts
├── tsconfig.json               # TypeScript configuration
├── vite.config.ts              # Vite configuration
├── .env.example                # Environment template
├── .env.development            # Development environment variables
└── README.md                   # This file
```

## CSS Variables & Design System

All styles use CSS custom properties for easy theming:

```css
--primary-color: #1f2937
--secondary-color: #3b82f6
--text-primary: #1f2937
--text-secondary: #6b7280
--bg-primary: #ffffff
--border-color: #e5e7eb
--spacing-md: 1rem
--font-base: 1rem
/* And more... */
```

Edit `src/styles/global.css` to customize the design system.

## Getting Started

### Prerequisites

- Node.js 16+ and npm/yarn
- Git

### Installation

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Set up environment variables**
   ```bash
   cp .env.example .env.development
   # Edit .env.development and set your API base URL
   ```

### Development

Start the development server:
```bash
npm run dev
```

The app will open at `http://localhost:5173`

### Build

Create production build:
```bash
npm run build
```

Preview production build:
```bash
npm run preview
```

## Key Features

### ✅ Complete Project Foundation
- Vite configuration with path aliases for clean imports
- React Router setup with all necessary routes
- React Query configured with sensible defaults
- TypeScript strict mode enabled

### ✅ Reusable Components
- `LoadingSpinner` - Animated loading indicator
- `EmptyState` - Placeholder for empty data
- `ErrorBoundary` - Error display with retry
- `Alert` - Toast-like notifications
- `Button` - Styled button component (primary, secondary, danger variants)
- `Badge` - Status badges

### ✅ API Integration
- Axios client with base URL configuration from environment
- Error handling with typed `ApiError` class
- Request/response types with TypeScript
- React Query hooks (`useApi`, `useFetch`, `usePaginatedApi`)
- Support for paginated API responses

### ✅ Type Safety
- Comprehensive TypeScript types for all API responses
- Enums for Order Status and API Error types
- Type-safe API endpoint constants
- Proper typing of React components and hooks

### ✅ Utility Functions
- `formatCurrency()` - Format prices in INR
- `formatDate()` / `formatDateTime()` - Date formatting
- `truncate()` - String truncation
- Email validation, discount calculation, and more

### ✅ Clean Architecture
- Separation of concerns (components, services, types, utils)
- Reusable hooks for API calls
- Centralized constants (routes, endpoints, messages)
- CSS custom properties for consistent theming

## Environment Variables

Create `.env.development` file:

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

For production, create `.env.production`:
```env
VITE_API_BASE_URL=https://api.example.com
```

## Available Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Start development server |
| `npm run build` | Build for production |
| `npm run preview` | Preview production build |
| `npm run lint` | Run ESLint (if configured) |

## API Integration Ready

The frontend is ready to integrate with your backend API:

1. **Update API endpoints** in `src/utils/constants.ts`
2. **Add type definitions** in `src/types/` directory
3. **Create API methods** using the `apiClient` from `src/services/api.ts`
4. **Use React Query hooks** in your components

Example:
```typescript
// In a component
const { data, isLoading, error } = useApi(
  QUERY_KEYS.PRODUCTS,
  API_ENDPOINTS.PRODUCTS
);
```

## Path Aliases

Use these aliases for cleaner imports:

```typescript
// Instead of:
import { something } from '../../../services/api'

// Use:
import { something } from '@services/api'
```

Available aliases:
- `@components/*` → `src/components/*`
- `@pages/*` → `src/pages/*`
- `@services/*` → `src/services/*`
- `@types/*` → `src/types/*`
- `@utils/*` → `src/utils/*`
- `@hooks/*` → `src/hooks/*`
- `@styles/*` → `src/styles/*`

## Responsive Design

The application is mobile-first with breakpoints at 768px. All components include responsive CSS.

## Next Steps

1. **Implement Product Pages**
   - Replace placeholders in `ProductListPage.tsx` and `ProductDetailsPage.tsx`
   - Create `ProductCard` component
   - Integrate with product API endpoints

2. **Implement Cart Management**
   - Add cart state/context or use React Query mutations
   - Create cart item list component
   - Implement cart summary

3. **Implement Checkout**
   - Create checkout flow
   - Integrate with payment API
   - Handle order creation

4. **Add Authentication**
   - Implement login/signup pages
   - Add auth context or store
   - Protect routes

5. **Code Quality** (Optional)
   - Add ESLint configuration
   - Add Prettier for formatting
   - Add unit tests with Vitest
   - Add E2E tests with Cypress/Playwright

6. **UI Enhancement** (Optional)
   - Add animations with Framer Motion
   - Add form handling with React Hook Form
   - Add validation with Zod

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## License

© 2024 IPL Merchandise Store. All rights reserved.
