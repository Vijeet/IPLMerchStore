import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { LoadingSpinner, ErrorBoundary, Button } from '@components/shared';
import { useProductDetails } from '@hooks/useProductDetails';
import { PRODUCT_TYPE_LABELS } from '@types/product';
import { formatCurrency } from '@utils/formatters';
import { ROUTES } from '@utils/constants';

export const ProductDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [quantity, setQuantity] = useState(1);
  const [addedToCart, setAddedToCart] = useState(false);

  const { data: product, isLoading, error } = useProductDetails(id);

  const handleQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = Math.max(1, Math.min(10, parseInt(e.target.value) || 1));
    setQuantity(value);
  };

  const handleAddToCart = () => {
    if (!product) return;

    // TODO: Integrate with cart module when available
    console.log(`Adding ${quantity} of product ${product.id} (${product.name}) to cart`);

    // Stub: Show success message
    setAddedToCart(true);
    setTimeout(() => setAddedToCart(false), 3000);
  };

  const handleBuyNow = () => {
    if (!product) return;

    // TODO: Integrate with checkout when available
    console.log(`Buying ${quantity} of product ${product.id}. Navigating to checkout...`);

    // Stub: For now, just navigate to cart
    navigate(ROUTES.CART);
  };

  if (isLoading) {
    return <LoadingSpinner message="Loading product details..." />;
  }

  if (error || !product) {
    return (
      <ErrorBoundary
        error={error || new Error('Product not found')}
        onRetry={() => {
          window.location.reload();
        }}
      />
    );
  }

  const isOutOfStock = product.inventoryCount <= 0;

  return (
    <div style={{ padding: '2rem 0' }}>
      {/* Back Button */}
      <button
        onClick={() => navigate(ROUTES.PRODUCTS)}
        style={{
          background: 'none',
          border: 'none',
          color: 'var(--secondary-color)',
          cursor: 'pointer',
          marginBottom: '2rem',
          fontSize: '1rem',
          transition: 'opacity 0.2s',
        }}
        onMouseEnter={(e) => {
          e.currentTarget.style.opacity = '0.7';
        }}
        onMouseLeave={(e) => {
          e.currentTarget.style.opacity = '1';
        }}
      >
        ← Back to Products
      </button>

      {/* Product Details Grid */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '3rem', marginBottom: '4rem' }}>
        {/* Image Section */}
        <div>
          <div
            style={{
              width: '100%',
              paddingBottom: '100%',
              position: 'relative',
              backgroundColor: 'var(--bg-secondary)',
              borderRadius: 'var(--radius-lg)',
              overflow: 'hidden',
              marginBottom: '1rem',
            }}
          >
            <img
              src={product.imageUrl || 'https://via.placeholder.com/500x500?text=No+Image'}
              alt={product.name}
              style={{
                position: 'absolute',
                top: 0,
                left: 0,
                width: '100%',
                height: '100%',
                objectFit: 'cover',
              }}
            />
          </div>

          {/* Stock Badge */}
          {isOutOfStock ? (
            <div
              style={{
                backgroundColor: 'var(--error-color)',
                color: 'white',
                padding: '0.75rem 1rem',
                borderRadius: 'var(--radius-md)',
                textAlign: 'center',
                fontWeight: 600,
              }}
            >
              Out of Stock
            </div>
          ) : (
            <div
              style={{
                backgroundColor: '#10b981',
                color: 'white',
                padding: '0.75rem 1rem',
                borderRadius: 'var(--radius-md)',
                textAlign: 'center',
                fontWeight: 600,
              }}
            >
              {product.inventoryCount} in Stock
            </div>
          )}
        </div>

        {/* Details Section */}
        <div>
          {/* Header Info */}
          <div style={{ marginBottom: '2rem' }}>
            <h1 style={{ fontSize: '2rem', fontWeight: 700, marginBottom: '0.5rem' }}>
              {product.name}
            </h1>
            <p style={{ color: 'var(--text-secondary)', marginBottom: '1rem' }}>
              SKU: {product.sku}
            </p>
          </div>

          {/* Meta Information */}
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: '1fr 1fr',
              gap: '1rem',
              marginBottom: '2rem',
              backgroundColor: 'var(--bg-secondary)',
              padding: '1.5rem',
              borderRadius: 'var(--radius-lg)',
            }}
          >
            <div>
              <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginBottom: '0.25rem' }}>
                Franchise
              </div>
              <div style={{ fontWeight: 600, fontSize: '1.1rem' }}>
                {product.franchiseName}
              </div>
              {product.franchiseShortCode && (
                <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: '0.25rem' }}>
                  ({product.franchiseShortCode})
                </div>
              )}
            </div>

            <div>
              <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginBottom: '0.25rem' }}>
                Product Type
              </div>
              <div style={{ fontWeight: 600, fontSize: '1.1rem' }}>
                {product.productType}
              </div>
            </div>
          </div>

          {/* Price */}
          <div style={{ marginBottom: '2rem' }}>
            <div
              style={{
                fontSize: '3rem',
                fontWeight: 700,
                color: 'var(--primary-color)',
                marginBottom: '0.5rem',
              }}
            >
              {formatCurrency(product.price, product.currency)}
            </div>
            <div style={{ color: 'var(--text-secondary)' }}>
              Inclusive of all taxes
            </div>
          </div>

          {/* Description */}
          <div style={{ marginBottom: '2rem' }}>
            <h3 style={{ fontSize: '1.1rem', fontWeight: 600, marginBottom: '0.75rem' }}>
              Description
            </h3>
            <p style={{ color: 'var(--text-secondary)', lineHeight: '1.6' }}>
              {product.description || 'No description available.'}
            </p>
          </div>

          {/* Quantity Selector */}
          <div style={{ marginBottom: '2rem' }}>
            <label
              htmlFor="quantity"
              style={{
                display: 'block',
                fontWeight: 600,
                marginBottom: '0.5rem',
              }}
            >
              Quantity
            </label>
            <input
              id="quantity"
              type="number"
              min="1"
              max="10"
              value={quantity}
              onChange={handleQuantityChange}
              disabled={isOutOfStock}
              style={{
                width: '5rem',
                padding: '0.5rem',
                fontSize: '1rem',
                border: '1px solid var(--border-color)',
                borderRadius: 'var(--radius-md)',
                backgroundColor: 'var(--bg-primary)',
                color: 'var(--text-primary)',
                cursor: isOutOfStock ? 'not-allowed' : 'number',
                opacity: isOutOfStock ? 0.5 : 1,
              }}
            />
          </div>

          {/* Action Buttons */}
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: '1fr 1fr',
              gap: '1rem',
              marginBottom: '2rem',
            }}
          >
            <Button
              onClick={handleAddToCart}
              disabled={isOutOfStock}
              variant="secondary"
            >
              🛒 Add to Cart
            </Button>
            <Button
              onClick={handleBuyNow}
              disabled={isOutOfStock}
              variant="primary"
            >
              💳 Buy Now
            </Button>
          </div>

          {/* Success Message */}
          {addedToCart && (
            <div
              style={{
                backgroundColor: '#d1fae5',
                color: '#065f46',
                padding: '1rem',
                borderRadius: 'var(--radius-md)',
                marginBottom: '1rem',
              }}
            >
              ✓ Added {quantity} item(s) to cart
            </div>
          )}

          {/* Additional Info */}
          <div
            style={{
              backgroundColor: 'var(--bg-secondary)',
              padding: '1rem',
              borderRadius: 'var(--radius-md)',
              fontSize: '0.9rem',
              color: 'var(--text-secondary)',
            }}
          >
            <div style={{ marginBottom: '0.5rem' }}>
              <strong>Product Code:</strong> {product.id}
            </div>
            <div style={{ marginBottom: '0.5rem' }}>
              <strong>Status:</strong> {product.isActive ? 'Active' : 'Inactive'}
            </div>
            <div>
              <strong>Added:</strong> {new Date(product.createdAtUtc).toLocaleDateString()}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
