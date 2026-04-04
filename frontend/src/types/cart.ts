/** Matches backend CartItemResponse */
export type CartItem = {
  id: number;
  productId: number;
  productName: string;
  productImageUrl: string;
  productSku: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  currentInventory: number | null;
  isProductActive: boolean;
};

/** Matches backend CartResponse */
export type Cart = {
  id: number;
  userId: string;
  items: CartItem[];
  itemCount: number;
  totalQuantity: number;
  totalAmount: number;
  currency: string;
  isEmpty: boolean;
};

/** Matches backend AddCartItemRequest */
export type AddToCartRequest = {
  productId: number;
  quantity: number;
};

/** Matches backend UpdateCartItemRequest */
export type UpdateCartItemRequest = {
  quantity: number;
};
