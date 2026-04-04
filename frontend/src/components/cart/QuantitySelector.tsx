import React from 'react';

interface QuantitySelectorProps {
  quantity: number;
  onIncrease: () => void;
  onDecrease: () => void;
  disabled?: boolean;
  max?: number;
}

export const QuantitySelector: React.FC<QuantitySelectorProps> = ({
  quantity,
  onIncrease,
  onDecrease,
  disabled = false,
  max,
}) => {
  const btnStyle: React.CSSProperties = {
    width: '2rem',
    height: '2rem',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    border: '1px solid var(--border-color)',
    borderRadius: 'var(--radius-md)',
    backgroundColor: 'var(--bg-secondary)',
    color: 'var(--text-primary)',
    cursor: disabled ? 'not-allowed' : 'pointer',
    opacity: disabled ? 0.5 : 1,
    fontSize: '1rem',
    fontWeight: 600,
  };

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
      <button
        onClick={onDecrease}
        disabled={disabled || quantity <= 1}
        style={{
          ...btnStyle,
          opacity: disabled || quantity <= 1 ? 0.4 : 1,
          cursor: disabled || quantity <= 1 ? 'not-allowed' : 'pointer',
        }}
        aria-label="Decrease quantity"
      >
        −
      </button>
      <span
        style={{
          minWidth: '2rem',
          textAlign: 'center',
          fontWeight: 600,
          fontSize: '1rem',
        }}
      >
        {quantity}
      </span>
      <button
        onClick={onIncrease}
        disabled={disabled || (max !== undefined && quantity >= max)}
        style={{
          ...btnStyle,
          opacity: disabled || (max !== undefined && quantity >= max) ? 0.4 : 1,
          cursor:
            disabled || (max !== undefined && quantity >= max)
              ? 'not-allowed'
              : 'pointer',
        }}
        aria-label="Increase quantity"
      >
        +
      </button>
    </div>
  );
};
