import React from 'react';
import { Franchise } from '@types/franchise';

interface FranchiseFilterProps {
  franchises: Franchise[];
  selectedFranchiseId?: number;
  onChange: (franchiseId: number | undefined) => void;
  isLoading?: boolean;
}

/**
 * FranchiseFilter component - dropdown filter for franchises
 */
export const FranchiseFilter: React.FC<FranchiseFilterProps> = ({
  franchises,
  selectedFranchiseId,
  onChange,
  isLoading = false,
}) => {
  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    onChange(value ? parseInt(value, 10) : undefined);
  };

  return (
    <div style={{ marginBottom: '1rem' }}>
      <label
        htmlFor="franchise-filter"
        style={{
          display: 'block',
          marginBottom: '0.5rem',
          fontWeight: 500,
          color: 'var(--text-primary)',
        }}
      >
        Franchise
      </label>
      <select
        id="franchise-filter"
        value={selectedFranchiseId || ''}
        onChange={handleChange}
        disabled={isLoading}
        style={{
          width: '100%',
          padding: '0.5rem',
          fontSize: '0.95rem',
          border: '1px solid var(--border-color)',
          borderRadius: 'var(--radius-md)',
          backgroundColor: 'var(--bg-primary)',
          color: 'var(--text-primary)',
          cursor: isLoading ? 'not-allowed' : 'pointer',
          opacity: isLoading ? 0.6 : 1,
        }}
      >
        <option value="">All Franchises</option>
        {franchises.map((franchise) => (
          <option key={franchise.id} value={franchise.id}>
            {franchise.name}
          </option>
        ))}
      </select>
    </div>
  );
};
