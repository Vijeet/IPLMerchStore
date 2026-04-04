export enum OrderStatus {
  Pending = 'PENDING',
  Processing = 'PROCESSING',
  Completed = 'COMPLETED',
  Cancelled = 'CANCELLED',
}

export type OrderItem = {
  productId: string;
  productName: string;
  price: number;
  quantity: number;
};

export type Order = {
  id: string;
  userId: string;
  items: OrderItem[];
  totalPrice: number;
  status: OrderStatus;
  paymentStatus: string;
  createdAt: string;
  updatedAt: string;
};

export type OrderDetail = Order & {
  shippingAddress?: string;
  notes?: string;
};

export type CheckoutRequest = {
  userId: string;
};

export type CreateOrderResponse = {
  orderId: string;
  totalPrice: number;
  status: OrderStatus;
};
