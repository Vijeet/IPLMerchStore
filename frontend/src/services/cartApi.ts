import { apiClient } from '@services/api';
import { Cart, AddToCartRequest, UpdateCartItemRequest } from '@appTypes/cart';
import { ApiError, ApiErrorType } from '@appTypes/common';
import { API_ENDPOINTS } from '@utils/constants';

const DEMO_USER_ID = 'demo-user-1';

const emptyCart = (userId: string): Cart => ({
  id: 0,
  userId,
  items: [],
  itemCount: 0,
  totalQuantity: 0,
  totalAmount: 0,
  currency: 'INR',
  isEmpty: true,
});

export const getCart = async (userId: string = DEMO_USER_ID): Promise<Cart> => {
  try {
    const response = await apiClient.get<Cart>(API_ENDPOINTS.CART(userId));
    if (!response.success || !response.data) {
      throw new Error(response.message || 'Failed to fetch cart');
    }
    return response.data;
  } catch (err) {
    if (err instanceof ApiError && err.type === ApiErrorType.NotFound) {
      return emptyCart(userId);
    }
    throw err;
  }
};

export const addToCart = async (
  request: AddToCartRequest,
  userId: string = DEMO_USER_ID
): Promise<Cart> => {
  const response = await apiClient.post<Cart>(
    API_ENDPOINTS.ADD_TO_CART(userId),
    request
  );
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to add item to cart');
  }
  return response.data;
};

export const updateCartItem = async (
  productId: number,
  request: UpdateCartItemRequest,
  userId: string = DEMO_USER_ID
): Promise<Cart> => {
  const response = await apiClient.put<Cart>(
    API_ENDPOINTS.UPDATE_CART_ITEM(userId, String(productId)),
    request
  );
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to update cart item');
  }
  return response.data;
};

export const removeCartItem = async (
  productId: number,
  userId: string = DEMO_USER_ID
): Promise<Cart> => {
  const response = await apiClient.delete<Cart>(
    API_ENDPOINTS.REMOVE_CART_ITEM(userId, String(productId))
  );
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to remove cart item');
  }
  return response.data;
};

export const clearCart = async (userId: string): Promise<void> => {
  await apiClient.delete(API_ENDPOINTS.CLEAR_CART(userId));
};

export const migrateGuestCart = async (targetUserId: string): Promise<boolean> => {
  const guestCart = await getCart('guest');
  if (guestCart.isEmpty || guestCart.items.length === 0) {
    return false;
  }
  for (const item of guestCart.items) {
    await addToCart({ productId: item.productId, quantity: item.quantity }, targetUserId);
  }
  try {
    await clearCart('guest');
  } catch {
    // Ignore clear failures
  }
  return true;
};
