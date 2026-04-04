/** Matches backend OrderStatus enum (int values 1-7) */
export enum OrderStatus {
  Pending = 1,
  Confirmed = 2,
  Processing = 3,
  Shipped = 4,
  Delivered = 5,
  Cancelled = 6,
  Returned = 7,
}

export const ORDER_STATUS_LABELS: Record<number, string> = {
  [OrderStatus.Pending]: 'Pending',
  [OrderStatus.Confirmed]: 'Confirmed',
  [OrderStatus.Processing]: 'Processing',
  [OrderStatus.Shipped]: 'Shipped',
  [OrderStatus.Delivered]: 'Delivered',
  [OrderStatus.Cancelled]: 'Cancelled',
  [OrderStatus.Returned]: 'Returned',
};

/** Matches backend OrderItemDto */
export type OrderItem = {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  subTotal: number;
};

/** Matches backend OrderDto */
export type Order = {
  id: number;
  userId: string;
  totalAmount: number;
  status: number;
  shippingAddress?: string;
  customerEmail?: string;
  customerPhone?: string;
  createdAtUtc: string;
  items: OrderItem[];
};

/** Matches backend CheckoutRequestDto (body) */
export type CheckoutRequest = {
  shippingAddress?: string;
  customerEmail?: string;
  customerPhone?: string;
};
