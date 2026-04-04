import React from 'react';
import { PRODUCT_TYPE_LABELS } from '@types/product';

interface ProductTypeFilterProps {
  selectedType?: number;
  onChange: (type: number | undefined) => void;
}

/**
 * ProductTypeFilter component - checkbox/radio filter for product types
 */
export const ProductTypeFilter: React.FC<ProductTypeFilterProps> = ({
  selectedType,
  onChange,
}) => {
  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    onChange(value ? parseInt(value, 10) : undefined);
  };

  return (
    <div style={{ marginBottom: '1rem' }}>
      <label
        htmlFor="type-filter"
        style={{
          display: 'block',
          marginBottom: '0.5rem',
          fontWeight: 500,
          color: 'var(--text-primary)',
        }}
      >
        Product Type
      </label>
      <select
        id="type-filter"
        value={selectedType || ''}
        onChange={handleChange}
        style={{
          width: '100%',
          padding: '0.5rem',
          fontSize: '0.95rem',
          border: '1px solid var(--border-color)',
          borderRadius: 'var(--radius-md)',
          backgroundColor: 'var(--bg-primary)',
          color: 'var(--text-primary)',
          cursor: 'pointer',
        }}
      >
        <option value="">All Types</option>
        {Object.entries(PRODUCT_TYPE_LABELS).map(([typeId, label]) => (
          <option key={typeId} value={typeId}>
            {label}
          </option>
        ))}
      </select>
    </div>
  );
};
