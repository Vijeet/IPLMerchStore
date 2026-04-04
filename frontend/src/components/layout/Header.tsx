import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { ROUTES } from '@utils/constants';
import { useCart } from '@hooks/useCart';
import { useAuth } from '@hooks/useAuth';
import './Header.css';

export const Header: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { cart } = useCart();
  const { isLoggedIn, email, logout } = useAuth();

  const isActive = (path: string): boolean => {
    return location.pathname === path || location.pathname.startsWith(path + '/');
  };

  const cartCount = cart?.totalQuantity ?? 0;

  return (
    <header className="header">
      <div className="header-container">
        <Link to={ROUTES.HOME} className="header-logo">
          <span>🏏</span>
          <span>IPL Merch Store</span>
        </Link>

        <nav className="header-nav">
          <ul>
            <li>
              <Link
                to={ROUTES.PRODUCTS}
                className={`header-nav-link ${isActive(ROUTES.PRODUCTS) ? 'active' : ''}`}
              >
                Products
              </Link>
            </li>
            <li>
              <Link
                to={ROUTES.CART}
                className={`header-nav-link ${isActive(ROUTES.CART) ? 'active' : ''}`}
              >
                Cart{cartCount > 0 && (
                  <span style={{
                    marginLeft: '0.35rem',
                    backgroundColor: 'var(--primary-color)',
                    color: 'white',
                    borderRadius: '999px',
                    padding: '0.1rem 0.45rem',
                    fontSize: '0.75rem',
                    fontWeight: 700,
                  }}>{cartCount}</span>
                )}
              </Link>
            </li>
            <li>
              <Link
                to={ROUTES.ORDERS}
                className={`header-nav-link ${isActive(ROUTES.ORDERS) ? 'active' : ''}`}
              >
                Orders
              </Link>
            </li>
            <li style={{ marginLeft: '1rem' }}>
              {isLoggedIn ? (
                <span style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontSize: '0.85rem' }}>
                  <span style={{ color: 'var(--text-secondary)' }}>{email}</span>
                  <button
                    onClick={() => { queryClient.clear(); logout(); navigate(ROUTES.HOME); }}
                    style={{
                      background: 'none',
                      border: '1px solid var(--border-color)',
                      borderRadius: 'var(--radius-md, 6px)',
                      color: 'var(--text-primary)',
                      cursor: 'pointer',
                      padding: '0.3rem 0.7rem',
                      fontSize: '0.8rem',
                    }}
                  >
                    Logout
                  </button>
                </span>
              ) : (
                <Link
                  to={ROUTES.LOGIN}
                  className={`header-nav-link ${isActive(ROUTES.LOGIN) ? 'active' : ''}`}
                >
                  Login
                </Link>
              )}
            </li>
          </ul>
        </nav>
      </div>
    </header>
  );
};
