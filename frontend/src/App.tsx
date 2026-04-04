import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AppLayout } from '@components/layout/AppLayout';
import { ToastProvider } from '@components/shared/Toast';
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

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ToastProvider>
      <Router>
        <AppLayout>
          <Routes>
            <Route path={ROUTES.HOME} element={<ProductListPage />} />
            <Route path={ROUTES.PRODUCTS} element={<ProductListPage />} />
            <Route path={ROUTES.PRODUCT_DETAILS} element={<ProductDetailsPage />} />
            <Route path={ROUTES.CART} element={<CartPage />} />
            <Route path={ROUTES.ORDERS} element={<OrderHistoryPage />} />
            <Route path={ROUTES.NOT_FOUND} element={<NotFoundPage />} />
            <Route path="*" element={<Navigate to={ROUTES.NOT_FOUND} replace />} />
          </Routes>
        </AppLayout>
      </Router>
      </ToastProvider>
    </QueryClientProvider>
  );
}

export default App;
