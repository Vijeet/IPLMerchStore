import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { ROUTES } from '@utils/constants';
import { useCart } from '@hooks/useCart';
import './Header.css';

export const Header: React.FC = () => {
  const location = useLocation();
  const { cart } = useCart();

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
          </ul>
        </nav>
      </div>
    </header>
  );
};
