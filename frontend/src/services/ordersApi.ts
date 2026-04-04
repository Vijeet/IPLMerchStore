import { apiClient } from '@services/api';
import { Order, CheckoutRequest } from '@appTypes/order';
import { PaginatedResponse } from '@appTypes/common';
import { API_ENDPOINTS } from '@utils/constants';

const DEMO_USER_ID = 'demo-user-1';

export const checkout = async (
  request: CheckoutRequest,
  userId: string = DEMO_USER_ID
): Promise<Order> => {
  const response = await apiClient.post<Order>(
    API_ENDPOINTS.CHECKOUT(userId),
    request
  );
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Checkout failed');
  }
  return response.data;
};

export const getUserOrders = async (
  userId: string = DEMO_USER_ID,
  pageNumber: number = 1,
  pageSize: number = 10
): Promise<PaginatedResponse<Order>> => {
  const url = `${API_ENDPOINTS.ORDERS(userId)}?pageNumber=${pageNumber}&pageSize=${pageSize}`;
  const response = await apiClient.get<PaginatedResponse<Order>>(url);
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to fetch orders');
  }
  return response.data;
};

export const getOrderDetail = async (
  orderId: number,
  userId: string = DEMO_USER_ID
): Promise<Order> => {
  const response = await apiClient.get<Order>(
    API_ENDPOINTS.ORDER_DETAILS(userId, String(orderId))
  );
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to fetch order details');
  }
  return response.data;
};

export const cancelOrder = async (
  orderId: number,
  userId: string = DEMO_USER_ID
): Promise<Order> => {
  const response = await apiClient.post<Order>(
    API_ENDPOINTS.CANCEL_ORDER(userId, String(orderId))
  );
  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to cancel order');
  }
  return response.data;
};
