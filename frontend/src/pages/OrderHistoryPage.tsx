import React from 'react';
import { useNavigate } from 'react-router-dom';
import { EmptyState } from '@components/shared';
import { ROUTES } from '@utils/constants';

export const OrderHistoryPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div style={{ padding: '2rem 0' }}>
      <h1>Order History</h1>
      <p style={{ color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
        View and track your orders
      </p>

      {/* Placeholder - Replace with actual orders */}
      <div style={{ marginTop: '2rem' }}>
        <EmptyState
          title="No orders yet"
          description="You haven't placed any orders yet. Start shopping to create your first order!"
          actionText="Start Shopping"
          onAction={() => navigate(ROUTES.PRODUCTS)}
          icon="📋"
        />
      </div>

      {/* Orders List - Ready for implementation */}
      {/* 
      <div style={{ marginTop: '2rem' }}>
        {orders.map(order => (
          <OrderCard key={order.id} order={order} />
        ))}
      </div>
      */}
    </div>
  );
};
