import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Cart } from '@types/cart';
import { formatCurrency } from '@utils/formatters';
import { Button } from '@components/shared';
import { ROUTES } from '@utils/constants';

interface CartSummaryProps {
  cart: Cart;
  isMutating: boolean;
}

export const CartSummary: React.FC<CartSummaryProps> = ({ cart, isMutating }) => {
  const navigate = useNavigate();

  return (
    <div
      style={{
        backgroundColor: 'var(--bg-secondary)',
        borderRadius: 'var(--radius-lg)',
        padding: '1.5rem',
        position: 'sticky',
        top: '6rem',
      }}
    >
      <h2 style={{ fontSize: '1.25rem', fontWeight: 700, marginBottom: '1.5rem' }}>
        Order Summary
      </h2>

      <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem', marginBottom: '1.5rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', color: 'var(--text-secondary)' }}>
          <span>Items ({cart.totalQuantity})</span>
          <span>{formatCurrency(cart.totalAmount, cart.currency)}</span>
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between', color: 'var(--text-secondary)' }}>
          <span>Shipping</span>
          <span style={{ color: '#10b981', fontWeight: 600 }}>FREE</span>
        </div>
        <hr style={{ border: 'none', borderTop: '1px solid var(--border-color)' }} />
        <div style={{ display: 'flex', justifyContent: 'space-between', fontWeight: 700, fontSize: '1.15rem' }}>
          <span>Total</span>
          <span style={{ color: 'var(--primary-color)' }}>
            {formatCurrency(cart.totalAmount, cart.currency)}
          </span>
        </div>
      </div>

      <Button
        variant="primary"
        disabled={isMutating || cart.isEmpty}
        onClick={() => navigate(ROUTES.ORDERS)}
        className=""
      >
        Proceed to Checkout
      </Button>

      <button
        onClick={() => navigate(ROUTES.PRODUCTS)}
        style={{
          width: '100%',
          marginTop: '0.75rem',
          background: 'none',
          border: 'none',
          color: 'var(--secondary-color)',
          cursor: 'pointer',
          fontSize: '0.9rem',
          padding: '0.5rem',
        }}
      >
        Continue Shopping
      </button>
    </div>
  );
};
