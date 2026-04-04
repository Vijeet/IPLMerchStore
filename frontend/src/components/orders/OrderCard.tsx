import React from 'react';
import { Order, ORDER_STATUS_LABELS } from '@appTypes/order';
import { formatCurrency } from '@utils/formatters';
import { formatDateTime } from '@utils/formatters';

interface OrderCardProps {
  order: Order;
}

const statusColor = (status: number): string => {
  switch (status) {
    case 5: return '#10b981'; // Delivered
    case 6: return '#ef4444'; // Cancelled
    case 7: return '#f59e0b'; // Returned
    case 4: return '#3b82f6'; // Shipped
    default: return '#6b7280'; // Pending/Processing/Confirmed
  }
};

export const OrderCard: React.FC<OrderCardProps> = ({ order }) => {
  const label = ORDER_STATUS_LABELS[order.status] || 'Unknown';

  return (
    <div
      style={{
        backgroundColor: 'var(--bg-secondary)',
        borderRadius: 'var(--radius-lg)',
        padding: '1.25rem',
        display: 'flex',
        flexDirection: 'column',
        gap: '0.75rem',
      }}
    >
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '0.5rem' }}>
        <div>
          <span style={{ fontWeight: 700, fontSize: '1rem' }}>Order #{order.id}</span>
          <span style={{ marginLeft: '1rem', fontSize: '0.85rem', color: 'var(--text-secondary)' }}>
            {formatDateTime(order.createdAtUtc)}
          </span>
        </div>
        <span
          style={{
            padding: '0.25rem 0.75rem',
            borderRadius: 'var(--radius-md)',
            fontSize: '0.8rem',
            fontWeight: 600,
            color: '#fff',
            backgroundColor: statusColor(order.status),
          }}
        >
          {label}
        </span>
      </div>

      {/* Items */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '0.4rem' }}>
        {order.items.map((item) => (
          <div
            key={item.id}
            style={{
              display: 'flex',
              justifyContent: 'space-between',
              fontSize: '0.9rem',
              color: 'var(--text-secondary)',
            }}
          >
            <span>
              {item.productName || `Product #${item.productId}`} × {item.quantity}
            </span>
            <span>{formatCurrency(item.subTotal)}</span>
          </div>
        ))}
      </div>

      {/* Footer */}
      <div
        style={{
          display: 'flex',
          justifyContent: 'flex-end',
          borderTop: '1px solid var(--border-color)',
          paddingTop: '0.75rem',
        }}
      >
        <span style={{ fontWeight: 700, fontSize: '1.05rem', color: 'var(--primary-color)' }}>
          Total: {formatCurrency(order.totalAmount)}
        </span>
      </div>
    </div>
  );
};
