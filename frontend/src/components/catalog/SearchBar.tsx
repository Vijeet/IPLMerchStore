import React, { useState, useEffect, useRef } from 'react';

interface SearchBarProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  onSearch?: (value: string) => void;
}

/**
 * SearchBar component with debounced input
 */
export const SearchBar: React.FC<SearchBarProps> = ({
  value,
  onChange,
  placeholder = 'Search products...',
  onSearch,
}) => {
  const [localValue, setLocalValue] = useState(value);
  const debounceTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  
  // Sync external value changes (e.g., when URL params change)
  useEffect(() => {
    setLocalValue(value);
    // Clear any pending debounce when external value changes
    if (debounceTimerRef.current) {
      clearTimeout(debounceTimerRef.current);
    }
  }, [value]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setLocalValue(newValue);

    // Clear existing timer
    if (debounceTimerRef.current) {
      clearTimeout(debounceTimerRef.current);
    }

    // Set new debounce timer
    debounceTimerRef.current = setTimeout(() => {
      onChange(newValue);
      debounceTimerRef.current = null;
    }, 300);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Clear pending timer and call onChange immediately
    if (debounceTimerRef.current) {
      clearTimeout(debounceTimerRef.current);
      debounceTimerRef.current = null;
    }
    onChange(localValue);
    if (onSearch) {
      onSearch(localValue);
    }
  };

  return (
    <form onSubmit={handleSubmit} style={{ marginBottom: '1.5rem' }}>
      <div style={{ position: 'relative' }}>
        <input
          type="text"
          value={localValue}
          onChange={handleChange}
          placeholder={placeholder}
          style={{
            width: '100%',
            padding: '0.75rem 2.5rem 0.75rem 1rem',
            fontSize: '1rem',
            border: '1px solid var(--border-color)',
            borderRadius: 'var(--radius-md)',
            backgroundColor: 'var(--bg-primary)',
            color: 'var(--text-primary)',
            transition: 'border-color 0.2s',
            boxSizing: 'border-box',
          }}
          onFocus={(e) => {
            e.currentTarget.style.borderColor = 'var(--primary-color)';
          }}
          onBlur={(e) => {
            e.currentTarget.style.borderColor = 'var(--border-color)';
          }}
        />
        <button
          type="submit"
          style={{
            position: 'absolute',
            right: '0.75rem',
            top: '50%',
            transform: 'translateY(-50%)',
            background: 'none',
            border: 'none',
            cursor: 'pointer',
            fontSize: '1.2rem',
            color: 'var(--text-secondary)',
          }}
        >
          🔍
        </button>
      </div>
    </form>
  );
};
