export type CartItem = {
  productId: string;
  productName: string;
  price: number;
  quantity: number;
  imageUrl: string;
};

export type Cart = {
  userId: string;
  items: CartItem[];
  totalPrice: number;
  createdAt: string;
  updatedAt: string;
};

export type AddToCartRequest = {
  productId: string;
  quantity: number;
};

export type UpdateCartItemRequest = {
  quantity: number;
};
