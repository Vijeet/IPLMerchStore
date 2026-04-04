import React from 'react';
import './shared.css';

export const LoadingSpinner: React.FC<{ message?: string }> = ({ message = 'Loading...' }) => {
  return (
    <div className="loading-container">
      <div className="spinner"></div>
      <p className="loading-message">{message}</p>
    </div>
  );
};

export const EmptyState: React.FC<{
  title: string;
  description?: string;
  actionText?: string;
  onAction?: () => void;
  icon?: string;
}> = ({ title, description, actionText, onAction, icon = '📦' }) => {
  return (
    <div className="empty-state">
      <div className="empty-state-icon">{icon}</div>
      <h2 className="empty-state-title">{title}</h2>
      {description && <p className="empty-state-description">{description}</p>}
      {actionText && onAction && (
        <button className="empty-state-button" onClick={onAction}>
          {actionText}
        </button>
      )}
    </div>
  );
};

export const ErrorBoundary: React.FC<{
  error: Error | string;
  onRetry?: () => void;
}> = ({ error, onRetry }) => {
  const errorMessage = typeof error === 'string' ? error : error.message;

  return (
    <div className="error-container">
      <div className="error-icon">⚠️</div>
      <h2 className="error-title">Something went wrong</h2>
      <p className="error-message">{errorMessage}</p>
      {onRetry && (
        <button className="error-button" onClick={onRetry}>
          Try Again
        </button>
      )}
    </div>
  );
};

export const Alert: React.FC<{
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  onClose?: () => void;
}> = ({ type, message, onClose }) => {
  return (
    <div className={`alert alert-${type}`}>
      <span className="alert-message">{message}</span>
      {onClose && (
        <button className="alert-close" onClick={onClose}>
          ✕
        </button>
      )}
    </div>
  );
};

export const Button: React.FC<{
  children: React.ReactNode;
  onClick?: () => void;
  variant?: 'primary' | 'secondary' | 'danger';
  disabled?: boolean;
  loading?: boolean;
  className?: string;
  type?: 'button' | 'submit' | 'reset';
}> = ({ 
  children, 
  onClick, 
  variant = 'primary', 
  disabled = false, 
  loading = false,
  className = '',
  type = 'button'
}) => {
  return (
    <button
      type={type}
      className={`btn btn-${variant} ${className}`}
      onClick={onClick}
      disabled={disabled || loading}
    >
      {loading ? '⏳ ' : ''}
      {children}
    </button>
  );
};

export const Badge: React.FC<{
  children: React.ReactNode;
  variant?: 'primary' | 'success' | 'warning' | 'error';
}> = ({ children, variant = 'primary' }) => {
  return <span className={`badge badge-${variant}`}>{children}</span>;
};
