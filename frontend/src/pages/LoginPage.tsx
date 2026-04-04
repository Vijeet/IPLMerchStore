import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '@hooks/useAuth';
import { Button } from '@components/shared';
import { ROUTES } from '@utils/constants';
import { migrateGuestCart } from '@services/cartApi';

export const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [isLoggingIn, setIsLoggingIn] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const from = (location.state as { from?: string })?.from || ROUTES.PRODUCTS;
  const isFromCart = from === ROUTES.CART;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = email.trim();
    if (!trimmed) {
      setError('Please enter your email address');
      return;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmed)) {
      setError('Please enter a valid email address');
      return;
    }
    setError('');
    setIsLoggingIn(true);
    login(trimmed);
    try {
      await migrateGuestCart(trimmed);
    } catch {
      // Non-critical — continue even if migration fails
    }
    setIsLoggingIn(false);
    navigate(from, { replace: true });
  };

  return (
    <div style={{ padding: '4rem 0', maxWidth: '400px', margin: '0 auto' }}>
      <div
        style={{
          backgroundColor: 'var(--bg-secondary)',
          borderRadius: 'var(--radius-lg)',
          padding: '2rem',
        }}
      >
        <h1 style={{ fontSize: '1.5rem', marginBottom: '0.5rem', textAlign: 'center' }}>
          Welcome to IPL Merch Store
        </h1>
        <p style={{ color: 'var(--text-secondary)', textAlign: 'center', marginBottom: '1.5rem', fontSize: '0.9rem' }}>
          {isFromCart
            ? 'Enter your email to view your cart and continue shopping'
            : 'Enter your email to continue'}
        </p>

        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
          <div>
            <label
              htmlFor="login-email"
              style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '0.3rem' }}
            >
              Email Address
            </label>
            <input
              id="login-email"
              type="email"
              placeholder="you@example.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              autoFocus
              style={{
                width: '100%',
                padding: '0.7rem 0.75rem',
                borderRadius: 'var(--radius-md, 8px)',
                border: `1px solid ${error ? '#ef4444' : 'var(--border-color, #ccc)'}`,
                fontSize: '1rem',
                backgroundColor: 'var(--bg-primary, #fff)',
                color: 'var(--text-primary, #222)',
                boxSizing: 'border-box',
              }}
            />
            {error && (
              <p style={{ fontSize: '0.8rem', color: '#ef4444', marginTop: '0.3rem' }}>{error}</p>
            )}
          </div>

          <Button type="submit" variant="primary" loading={isLoggingIn} disabled={isLoggingIn}>
            {isLoggingIn ? 'Logging in...' : 'Continue'}
          </Button>
        </form>
      </div>
    </div>
  );
};
