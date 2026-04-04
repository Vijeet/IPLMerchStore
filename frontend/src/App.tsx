import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AppLayout } from '@components/layout/AppLayout';
import { ProductListPage } from '@pages/ProductListPage';
import { ProductDetailsPage } from '@pages/ProductDetailsPage';
import { CartPage } from '@pages/CartPage';
import { OrderHistoryPage } from '@pages/OrderHistoryPage';
import { NotFoundPage } from '@pages/NotFoundPage';
import { ROUTES } from '@utils/constants';
import './App.css';

// Create a client for React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

const HomePage: React.FC = () => (
  <div className="home-page">
    <div className="home-header">
      <h1>Welcome to IPL Merchandise Store</h1>
      <p>Your one-stop shop for official IPL merchandise</p>
    </div>

    <div className="home-grid">
      <div className="home-card">
        <div className="home-card-icon">🛍️</div>
        <h3>Explore Products</h3>
        <p>Browse our wide collection of official IPL merchandise</p>
        <a href={ROUTES.PRODUCTS} className="home-card-link">
          Shop Now →
        </a>
      </div>

      <div className="home-card">
        <div className="home-card-icon">🛒</div>
        <h3>Easy Checkout</h3>
        <p>Add items to cart and checkout seamlessly</p>
        <a href={ROUTES.CART} className="home-card-link">
          View Cart →
        </a>
      </div>

      <div className="home-card">
        <div className="home-card-icon">📦</div>
        <h3>Track Orders</h3>
        <p>Monitor your orders and deliveries in real-time</p>
        <a href={ROUTES.ORDERS} className="home-card-link">
          My Orders →
        </a>
      </div>
    </div>
  </div>
);

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <AppLayout>
          <Routes>
            <Route path={ROUTES.HOME} element={<HomePage />} />
            <Route path={ROUTES.PRODUCTS} element={<ProductListPage />} />
            <Route path={ROUTES.PRODUCT_DETAILS} element={<ProductDetailsPage />} />
            <Route path={ROUTES.CART} element={<CartPage />} />
            <Route path={ROUTES.ORDERS} element={<OrderHistoryPage />} />
            <Route path={ROUTES.NOT_FOUND} element={<NotFoundPage />} />
            <Route path="*" element={<Navigate to={ROUTES.NOT_FOUND} replace />} />
          </Routes>
        </AppLayout>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
