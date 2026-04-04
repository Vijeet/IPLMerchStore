import React from 'react';
import { useNavigate } from 'react-router-dom';
import { ROUTES } from '@utils/constants';
import './pages.css';

export const NotFoundPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <div className="not-found-page">
      <h1 className="not-found-title">404</h1>
      <h2 className="not-found-heading">Page Not Found</h2>
      <p className="not-found-description">
        The page you're looking for doesn't exist or has been moved.
      </p>
      <button className="not-found-button" onClick={() => navigate(ROUTES.HOME)}>
        Go Home
      </button>
    </div>
  );
};
