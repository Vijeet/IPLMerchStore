import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Product, ProductSearchResult, PRODUCT_TYPE_LABELS } from '@types/product';
import { formatCurrency } from '@utils/formatters';
import { useCart } from '@hooks/useCart';
import { useToast } from '@components/shared/Toast';

interface ProductCardProps {
  product: Product | ProductSearchResult;
  onClick?: () => void;
}

/**
 * ProductCard component - displays product in grid
 */
export const ProductCard: React.FC<ProductCardProps> = ({ product, onClick }) => {
  const navigate = useNavigate();
  const { addToCart, isMutating } = useCart();
  const { showToast } = useToast();

  const handleClick = () => {
    if (onClick) {
      onClick();
    }
    navigate(`/products/${product.id}`);
  };

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.stopPropagation();
    try {
      await addToCart({ productId: product.id, quantity: 1 });
      showToast(`${product.name} added to cart`, 'success');
    } catch {
      showToast('Failed to add item to cart', 'error');
    }
  };

  const getProductTypeLabel = (product: Product | ProductSearchResult): string => {
    if ('productTypeLabel' in product) {
      return product.productTypeLabel;
    }
    return PRODUCT_TYPE_LABELS[product.productType] || 'Unknown';
  };

  const isOutOfStock = product.inventoryCount <= 0;

  return (
    <div
      onClick={handleClick}
      style={{
        display: 'flex',
        flexDirection: 'column',
        backgroundColor: 'var(--bg-secondary)',
        borderRadius: 'var(--radius-lg)',
        overflow: 'hidden',
        cursor: 'pointer',
        transition: 'transform 0.2s, box-shadow 0.2s',
        boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
        height: '100%',
      }}
      onMouseEnter={(e) => {
        const el = e.currentTarget;
        el.style.transform = 'translateY(-4px)';
        el.style.boxShadow = '0 8px 16px rgba(0, 0, 0, 0.15)';
      }}
      onMouseLeave={(e) => {
        const el = e.currentTarget;
        el.style.transform = 'translateY(0)';
        el.style.boxShadow = '0 2px 4px rgba(0, 0, 0, 0.1)';
      }}
    >
      {/* Image Section */}
      <div
        style={{
          position: 'relative',
          width: '100%',
          paddingBottom: '100%',
          backgroundColor: 'var(--bg-primary)',
          overflow: 'hidden',
        }}
      >
        <img
          src={product.imageUrl || 'https://via.placeholder.com/250x250?text=No+Image'}
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
        {isOutOfStock && (
          <div
            style={{
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: '100%',
              backgroundColor: 'rgba(0, 0, 0, 0.5)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: 'white',
              fontWeight: 'bold',
              fontSize: '1.2rem',
            }}
          >
            Out of Stock
          </div>
        )}
      </div>

      {/* Content Section */}
      <div
        style={{
          flex: 1,
          padding: '1rem',
          display: 'flex',
          flexDirection: 'column',
        }}
      >
        {/* Product Name */}
        <h3
          style={{
            margin: '0 0 0.5rem 0',
            fontSize: '1rem',
            fontWeight: 600,
            color: 'var(--text-primary)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          }}
        >
          {product.name}
        </h3>

        {/* Franchise and Type */}
        <div
          style={{
            display: 'flex',
            gap: '0.5rem',
            marginBottom: '0.5rem',
            fontSize: '0.8rem',
            color: 'var(--text-secondary)',
          }}
        >
          <span style={{ backgroundColor: 'var(--bg-primary)', padding: '0.2rem 0.5rem', borderRadius: '0.3rem' }}>
            {product.franchiseName}
          </span>
        </div>

        {/* Product Type */}
        <div
          style={{
            marginBottom: '0.75rem',
            fontSize: '0.85rem',
            color: 'var(--secondary-color)',
            fontWeight: 500,
          }}
        >
          {getProductTypeLabel(product)}
        </div>

        {/* Stock Info */}
        <div
          style={{
            marginBottom: '0.75rem',
            fontSize: '0.8rem',
            color: isOutOfStock ? 'var(--error-color)' : 'var(--text-secondary)',
          }}
        >
          {isOutOfStock ? 'Out of Stock' : `${product.inventoryCount} in stock`}
        </div>

        {/* Price */}
        <div
          style={{
            marginTop: 'auto',
            fontSize: '1.25rem',
            fontWeight: 700,
            color: 'var(--primary-color)',
          }}
        >
          {formatCurrency(product.price, product.currency)}
        </div>

        {/* Add to Cart */}
        {!isOutOfStock && (
          <button
            onClick={handleAddToCart}
            disabled={isMutating}
            style={{
              marginTop: '0.75rem',
              width: '100%',
              padding: '0.5rem',
              fontSize: '0.85rem',
              fontWeight: 600,
              border: '1px solid var(--primary-color)',
              borderRadius: 'var(--radius-md)',
              backgroundColor: 'transparent',
              color: 'var(--primary-color)',
              cursor: isMutating ? 'not-allowed' : 'pointer',
              opacity: isMutating ? 0.6 : 1,
              transition: 'background-color 0.2s, color 0.2s',
            }}
            onMouseEnter={(e) => {
              if (!isMutating) {
                e.currentTarget.style.backgroundColor = 'var(--primary-color)';
                e.currentTarget.style.color = 'white';
              }
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor = 'transparent';
              e.currentTarget.style.color = 'var(--primary-color)';
            }}
          >
            {isMutating ? '⏳ Adding...' : '🛒 Add to Cart'}
          </button>
        )}
      </div>
    </div>
  );
};
