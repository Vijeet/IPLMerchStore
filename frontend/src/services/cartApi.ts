import { apiClient } from '@services/api';
import { Cart, AddToCartRequest, UpdateCartItemRequest } from '@appTypes/cart';
import { API_ENDPOINTS } from '@utils/constants';

const DEMO_USER_ID = 'demo-user-1';

export const getCart = async (userId: string = DEMO_USER_ID): Promise<Cart> => {
  const response = await apiClient.get<Cart>(API_ENDPOINTS.CART(userId));
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to fetch cart');
  }
  return response.data;
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
