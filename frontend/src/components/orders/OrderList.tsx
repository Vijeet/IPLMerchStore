import React from 'react';
import { Order } from '@appTypes/order';
import { OrderCard } from './OrderCard';

interface OrderListProps {
  orders: Order[];
  onCancel?: (orderId: number) => Promise<unknown>;
  isCancelling?: boolean;
}

export const OrderList: React.FC<OrderListProps> = ({ orders, onCancel, isCancelling }) => {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
      {orders.map((order) => (
        <OrderCard key={order.id} order={order} onCancel={onCancel} isCancelling={isCancelling} />
      ))}
    </div>
  );
};
