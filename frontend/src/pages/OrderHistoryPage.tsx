import React from 'react';
import { useNavigate } from 'react-router-dom';
import { LoadingSpinner, EmptyState, ErrorBoundary } from '@components/shared';
import { OrderList } from '@components/orders';
import { useOrders, useCancelOrder } from '@hooks/useOrders';
import { ROUTES } from '@utils/constants';

export const OrderHistoryPage: React.FC = () => {
  const navigate = useNavigate();
  const { orders, isLoading, isError, error } = useOrders();
  const { cancelOrder, isCancelling } = useCancelOrder();

  if (isLoading) {
    return <LoadingSpinner message="Loading your orders..." />;
  }

  if (isError) {
    return (
      <ErrorBoundary
        error={error || new Error('Failed to load orders')}
        onRetry={() => window.location.reload()}
      />
    );
  }

  const orderItems = orders?.items ?? [];

  return (
    <div style={{ padding: '2rem 0' }}>
      <h1>Order History</h1>
      <p style={{ color: 'var(--text-secondary)', marginTop: '0.5rem', marginBottom: '2rem' }}>
        View and track your orders
      </p>

      {orderItems.length === 0 ? (
        <EmptyState
          title="No orders yet"
          description="You haven't placed any orders yet. Start shopping to create your first order!"
          actionText="Start Shopping"
          onAction={() => navigate(ROUTES.PRODUCTS)}
          icon="📋"
        />
      ) : (
        <OrderList orders={orderItems} onCancel={cancelOrder} isCancelling={isCancelling} />
      )}
    </div>
  );
};
