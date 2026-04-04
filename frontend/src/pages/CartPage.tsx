import React from 'react';
import { useNavigate } from 'react-router-dom';
import { EmptyState } from '@components/shared';
import { ROUTES } from '@utils/constants';

export const CartPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div style={{ padding: '2rem 0' }}>
      <h1>Shopping Cart</h1>
      <p style={{ color: 'var(--text-secondary)', marginTop: '0.5rem' }}>
        Review and manage your items
      </p>

      {/* Placeholder - Replace with actual cart */}
      <div style={{ marginTop: '2rem' }}>
        <EmptyState
          title="Your cart is empty"
          description="Start shopping to add items to your cart. Browse our amazing IPL merchandise collection!"
          actionText="Continue Shopping"
          onAction={() => navigate(ROUTES.PRODUCTS)}
          icon="🛒"
        />
      </div>

      {/* Cart Layout - Ready for implementation */}
      {/* 
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 350px', gap: '2rem', marginTop: '2rem' }}>
        <div>
          <CartItemList items={cartItems} />
        </div>
        <div>
          <CartSummary total={totalPrice} />
          <button>Proceed to Checkout</button>
        </div>
      </div>
      */}
    </div>
  );
};
