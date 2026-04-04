import React from 'react';
import { Header } from './Header';
import '@styles/AppLayout.css';

interface AppLayoutProps {
  children: React.ReactNode;
}

export const AppLayout: React.FC<AppLayoutProps> = ({ children }) => {
  return (
    <div className="layout">
      <Header />
      <main className="layout-main">
        <div className="layout-content">
          <div className="container">{children}</div>
        </div>
      </main>
      <footer className="layout-footer">
        <div className="container">
          <p style={{ textAlign: 'center', margin: 0, color: 'var(--text-secondary)' }}>
            &copy; 2024 IPL Merchandise Store. All rights reserved.
          </p>
        </div>
      </footer>
    </div>
  );
};
