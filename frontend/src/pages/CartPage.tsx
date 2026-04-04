import React from 'react';
import { useNavigate } from 'react-router-dom';
import { LoadingSpinner, EmptyState, ErrorBoundary, Button } from '@components/shared';
import { QuantitySelector, CartSummary } from '@components/cart';
import { useCart } from '@hooks/useCart';
import { useToast } from '@components/shared/Toast';
import { formatCurrency } from '@utils/formatters';
import { ROUTES } from '@utils/constants';

export const CartPage: React.FC = () => {
  const navigate = useNavigate();
  const {
    cart,
    isLoading,
    isError,
    error,
    updateItem,
    removeItem,
    isMutating,
  } = useCart();
  const { showToast } = useToast();

  if (isLoading) {
    return <LoadingSpinner message="Loading your cart..." />;
  }

  if (isError) {
    return (
      <ErrorBoundary
        error={error || new Error('Failed to load cart')}
        onRetry={() => window.location.reload()}
      />
    );
  }

  if (!cart || cart.isEmpty) {
    return (
      <div style={{ padding: '2rem 0' }}>
        <h1>Shopping Cart</h1>
        <div style={{ marginTop: '2rem' }}>
          <EmptyState
            title="Your cart is empty"
            description="Start shopping to add items to your cart. Browse our amazing IPL merchandise collection!"
            actionText="Continue Shopping"
            onAction={() => navigate(ROUTES.PRODUCTS)}
            icon="🛒"
          />
        </div>
      </div>
    );
  }

  return (
    <div style={{ padding: '2rem 0' }}>
      <h1>Shopping Cart</h1>
      <p style={{ color: 'var(--text-secondary)', marginTop: '0.5rem', marginBottom: '2rem' }}>
        {cart.totalQuantity} item{cart.totalQuantity !== 1 ? 's' : ''} in your cart
      </p>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 350px', gap: '2rem', alignItems: 'start' }}>
        {/* Cart Items */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
          {cart.items.map((item) => (
            <div
              key={item.productId}
              style={{
                display: 'flex',
                gap: '1rem',
                padding: '1rem',
                backgroundColor: 'var(--bg-secondary)',
                borderRadius: 'var(--radius-lg)',
                opacity: isMutating ? 0.7 : 1,
                transition: 'opacity 0.2s',
              }}
            >
              {/* Image */}
              <img
                src={item.productImageUrl || 'https://via.placeholder.com/100x100?text=No+Image'}
                alt={item.productName || 'Product'}
                onClick={() => navigate(`/products/${item.productId}`)}
                style={{
                  width: '100px',
                  height: '100px',
                  objectFit: 'cover',
                  borderRadius: 'var(--radius-md)',
                  cursor: 'pointer',
                  flexShrink: 0,
                }}
              />

              {/* Details */}
              <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '0.25rem' }}>
                <h3
                  style={{
                    margin: 0,
                    fontSize: '1rem',
                    fontWeight: 600,
                    cursor: 'pointer',
                  }}
                  onClick={() => navigate(`/products/${item.productId}`)}
                >
                  {item.productName}
                </h3>
                {item.productSku && (
                  <span style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>
                    SKU: {item.productSku}
                  </span>
                )}
                <span style={{ fontSize: '0.9rem', color: 'var(--text-secondary)' }}>
                  {formatCurrency(item.unitPrice, cart.currency)} each
                </span>

                <div style={{ marginTop: 'auto', display: 'flex', alignItems: 'center', gap: '1rem', paddingTop: '0.5rem' }}>
                  <QuantitySelector
                    quantity={item.quantity}
                    onIncrease={() => updateItem(item.productId, item.quantity + 1)}
                    onDecrease={() => updateItem(item.productId, item.quantity - 1)}
                    disabled={isMutating}
                    max={item.currentInventory ?? undefined}
                  />
                  <Button
                    variant="danger"
                    disabled={isMutating}
                    onClick={async () => {
                      const name = item.productName;
                      const qty = item.quantity;
                      try {
                        await removeItem(item.productId);
                        showToast(`Removed ${name} (×${qty}) from cart`, 'success');
                      } catch {
                        showToast(`Failed to remove ${name}`, 'error');
                      }
                    }}
                  >
                    Remove
                  </Button>
                </div>
              </div>

              {/* Subtotal */}
              <div
                style={{
                  fontWeight: 700,
                  fontSize: '1.1rem',
                  color: 'var(--primary-color)',
                  flexShrink: 0,
                  textAlign: 'right',
                  minWidth: '6rem',
                }}
              >
                {formatCurrency(item.subtotal, cart.currency)}
              </div>
            </div>
          ))}
        </div>

        {/* Summary */}
        <CartSummary cart={cart} isMutating={isMutating} />
      </div>
    </div>
  );
};
