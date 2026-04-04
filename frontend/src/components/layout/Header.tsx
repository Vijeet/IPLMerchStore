import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { ROUTES } from '@utils/constants';
import './Header.css';

export const Header: React.FC = () => {
  const location = useLocation();

  const isActive = (path: string): boolean => {
    return location.pathname === path || location.pathname.startsWith(path + '/');
  };

  return (
    <header className="header">
      <div className="header-container">
        <Link to={ROUTES.HOME} className="header-logo">
          <span>🏏</span>
          <span>IPL Merch Store</span>
        </Link>

        <nav className="header-nav">
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
              Cart
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
        </nav>
      </div>
    </header>
  );
};
