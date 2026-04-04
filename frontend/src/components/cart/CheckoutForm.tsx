import React, { useState } from 'react';
import { Button } from '@components/shared';
import { CheckoutRequest } from '@appTypes/order';
import { useAuth } from '@hooks/useAuth';

interface CheckoutFormProps {
  onSubmit: (details: CheckoutRequest) => void;
  onCancel: () => void;
  isSubmitting: boolean;
}

export const CheckoutForm: React.FC<CheckoutFormProps> = ({ onSubmit, onCancel, isSubmitting }) => {
  const { email: authEmail } = useAuth();
  const [email, setEmail] = useState(authEmail || '');
  const [address, setAddress] = useState('');
  const [phone, setPhone] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) {
      newErrors.email = 'Enter a valid email address';
    }

    if (!address.trim()) {
      newErrors.address = 'Shipping address is required';
    }

    if (!phone.trim()) {
      newErrors.phone = 'Contact number is required';
    } else if (!/^\+?[\d\s\-()]{7,15}$/.test(phone.trim())) {
      newErrors.phone = 'Enter a valid contact number';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validate()) {
      onSubmit({
        customerEmail: email.trim(),
        shippingAddress: address.trim(),
        customerPhone: phone.trim(),
      });
    }
  };

  const inputStyle: React.CSSProperties = {
    width: '100%',
    padding: '0.6rem 0.75rem',
    borderRadius: 'var(--radius-md, 8px)',
    border: '1px solid var(--border-color, #ccc)',
    fontSize: '0.9rem',
    backgroundColor: 'var(--bg-primary, #fff)',
    color: 'var(--text-primary, #222)',
    boxSizing: 'border-box',
  };

  const labelStyle: React.CSSProperties = {
    display: 'block',
    fontSize: '0.85rem',
    fontWeight: 600,
    marginBottom: '0.3rem',
    color: 'var(--text-primary, #222)',
  };

  const errorStyle: React.CSSProperties = {
    fontSize: '0.78rem',
    color: '#ef4444',
    marginTop: '0.2rem',
  };

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
      <h3 style={{ margin: 0, fontSize: '1.05rem', fontWeight: 700 }}>Delivery Details</h3>

      <div>
        <label style={labelStyle} htmlFor="checkout-email">Email Address</label>
        <input
          id="checkout-email"
          type="email"
          placeholder="you@example.com"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          style={{ ...inputStyle, borderColor: errors.email ? '#ef4444' : undefined }}
          disabled={isSubmitting}
        />
        {errors.email && <p style={errorStyle}>{errors.email}</p>}
      </div>

      <div>
        <label style={labelStyle} htmlFor="checkout-address">Shipping Address</label>
        <textarea
          id="checkout-address"
          placeholder="Full shipping address"
          value={address}
          onChange={(e) => setAddress(e.target.value)}
          rows={3}
          style={{ ...inputStyle, resize: 'vertical', borderColor: errors.address ? '#ef4444' : undefined }}
          disabled={isSubmitting}
        />
        {errors.address && <p style={errorStyle}>{errors.address}</p>}
      </div>

      <div>
        <label style={labelStyle} htmlFor="checkout-phone">Contact Number</label>
        <input
          id="checkout-phone"
          type="tel"
          placeholder="+91 98765 43210"
          value={phone}
          onChange={(e) => setPhone(e.target.value)}
          style={{ ...inputStyle, borderColor: errors.phone ? '#ef4444' : undefined }}
          disabled={isSubmitting}
        />
        {errors.phone && <p style={errorStyle}>{errors.phone}</p>}
      </div>

      <div style={{ display: 'flex', gap: '0.75rem', marginTop: '0.25rem' }}>
        <Button type="submit" variant="primary" disabled={isSubmitting} loading={isSubmitting}>
          {isSubmitting ? 'Placing Order...' : 'Confirm & Place Order'}
        </Button>
        <Button type="button" variant="secondary" disabled={isSubmitting} onClick={onCancel}>
          Cancel
        </Button>
      </div>
    </form>
  );
};
