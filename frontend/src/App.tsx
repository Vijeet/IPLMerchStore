import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AppLayout } from '@components/layout/AppLayout';
import { ToastProvider } from '@components/shared/Toast';
import { AuthProvider, useAuth } from '@hooks/useAuth';
import { ProductListPage } from '@pages/ProductListPage';
import { ProductDetailsPage } from '@pages/ProductDetailsPage';
import { CartPage } from '@pages/CartPage';
import { OrderHistoryPage } from '@pages/OrderHistoryPage';
import { LoginPage } from '@pages/LoginPage';
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

const RequireAuth: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isLoggedIn } = useAuth();
  const location = useLocation();
  if (!isLoggedIn) {
    return <Navigate to={ROUTES.LOGIN} state={{ from: location.pathname }} replace />;
  }
  return <>{children}</>;
};

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ToastProvider>
      <AuthProvider>
      <Router>
        <AppLayout>
          <Routes>
            <Route path={ROUTES.HOME} element={<ProductListPage />} />
            <Route path={ROUTES.LOGIN} element={<LoginPage />} />
            <Route path={ROUTES.PRODUCTS} element={<ProductListPage />} />
            <Route path={ROUTES.PRODUCT_DETAILS} element={<ProductDetailsPage />} />
            <Route path={ROUTES.CART} element={<RequireAuth><CartPage /></RequireAuth>} />
            <Route path={ROUTES.ORDERS} element={<RequireAuth><OrderHistoryPage /></RequireAuth>} />
            <Route path={ROUTES.NOT_FOUND} element={<NotFoundPage />} />
            <Route path="*" element={<Navigate to={ROUTES.NOT_FOUND} replace />} />
          </Routes>
        </AppLayout>
      </Router>
      </AuthProvider>
      </ToastProvider>
    </QueryClientProvider>
  );
}

export default App;
