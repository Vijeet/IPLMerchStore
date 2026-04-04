import React, { useState } from 'react';
import { Order, OrderStatus, ORDER_STATUS_LABELS } from '@appTypes/order';
import { formatCurrency, formatDateTime } from '@utils/formatters';
import { Button } from '@components/shared';

interface OrderCardProps {
  order: Order;
  onCancel?: (orderId: number) => Promise<unknown>;
  isCancelling?: boolean;
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

export const OrderCard: React.FC<OrderCardProps> = ({ order, onCancel, isCancelling }) => {
  const label = ORDER_STATUS_LABELS[order.status] || 'Unknown';
  const [showConfirm, setShowConfirm] = useState(false);
  const isPending = order.status === OrderStatus.Pending;

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
          justifyContent: 'space-between',
          alignItems: 'center',
          borderTop: '1px solid var(--border-color)',
          paddingTop: '0.75rem',
        }}
      >
        {isPending && onCancel && (
          <Button
            variant="danger"
            disabled={isCancelling}
            loading={isCancelling}
            onClick={() => setShowConfirm(true)}
          >
            Cancel Order
          </Button>
        )}
        {(!isPending || !onCancel) && <span />}
        <span style={{ fontWeight: 700, fontSize: '1.05rem', color: 'var(--primary-color)' }}>
          Total: {formatCurrency(order.totalAmount)}
        </span>
      </div>

      {/* Confirmation Dialog */}
      {showConfirm && (
        <div
          style={{
            position: 'fixed',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            zIndex: 9999,
          }}
          onClick={() => setShowConfirm(false)}
        >
          <div
            style={{
              backgroundColor: 'var(--bg-primary)',
              borderRadius: 'var(--radius-lg)',
              padding: '2rem',
              maxWidth: '400px',
              width: '90%',
              boxShadow: '0 8px 32px rgba(0, 0, 0, 0.2)',
            }}
            onClick={(e) => e.stopPropagation()}
          >
            <h3 style={{ margin: '0 0 0.75rem', fontSize: '1.1rem' }}>Cancel Order #{order.id}?</h3>
            <p style={{ color: 'var(--text-secondary)', margin: '0 0 1.5rem', fontSize: '0.9rem' }}>
              This action cannot be undone. The order items will be returned to inventory.
            </p>
            <div style={{ display: 'flex', gap: '0.75rem', justifyContent: 'flex-end' }}>
              <Button
                variant="secondary"
                onClick={() => setShowConfirm(false)}
                disabled={isCancelling}
              >
                Keep Order
              </Button>
              <Button
                variant="danger"
                loading={isCancelling}
                disabled={isCancelling}
                onClick={async () => {
                  try {
                    await onCancel!(order.id);
                  } finally {
                    setShowConfirm(false);
                  }
                }}
              >
                {isCancelling ? 'Cancelling...' : 'Yes, Cancel'}
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
