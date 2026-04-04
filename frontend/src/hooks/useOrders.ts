import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Order, CheckoutRequest } from '@appTypes/order';
import { PaginatedResponse, ApiError } from '@appTypes/common';
import { checkout, getUserOrders, getOrderDetail, cancelOrder } from '@services/ordersApi';
import { QUERY_KEYS, ROUTES } from '@utils/constants';
import { useToast } from '@components/shared/Toast';
import { useAuth } from '@hooks/useAuth';

export const useOrders = (pageNumber = 1, pageSize = 10) => {
  const { userId: authUserId, isLoggedIn } = useAuth();
  const userId = authUserId || 'guest';
  const ordersQuery = useQuery<PaginatedResponse<Order>, ApiError>({
    queryKey: [...QUERY_KEYS.ORDERS(userId), pageNumber, pageSize],
    queryFn: () => getUserOrders(userId, pageNumber, pageSize),
    enabled: isLoggedIn,
    retry: 1,
    staleTime: 60 * 1000,
  });

  return {
    orders: ordersQuery.data,
    isLoading: ordersQuery.isLoading,
    isError: ordersQuery.isError,
    error: ordersQuery.error,
  };
};

export const useOrderDetail = (orderId: number) => {
  const { userId: authUserId, isLoggedIn } = useAuth();
  const userId = authUserId || 'guest';
  const orderQuery = useQuery<Order, ApiError>({
    queryKey: QUERY_KEYS.ORDER(userId, String(orderId)),
    queryFn: () => getOrderDetail(orderId, userId),
    enabled: isLoggedIn && orderId > 0,
    retry: 1,
    staleTime: 60 * 1000,
  });

  return {
    order: orderQuery.data,
    isLoading: orderQuery.isLoading,
    isError: orderQuery.isError,
    error: orderQuery.error,
  };
};

export const useCheckout = () => {
  const { userId: authUserId } = useAuth();
  const userId = authUserId || 'guest';
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const { showToast } = useToast();

  const checkoutMutation = useMutation<Order, ApiError, CheckoutRequest>({
    mutationFn: (request) => checkout(request, userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.CART(userId) });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ORDERS(userId) });
      showToast('Order placed successfully!', 'success');
      navigate(ROUTES.ORDERS);
    },
    onError: (error) => {
      showToast(error.message || 'Checkout failed. Please try again.', 'error');
    },
  });

  return {
    checkout: checkoutMutation.mutateAsync,
    isCheckingOut: checkoutMutation.isPending,
    checkoutError: checkoutMutation.error,
  };
};

export const useCancelOrder = () => {
  const { userId: authUserId } = useAuth();
  const userId = authUserId || 'guest';
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  const cancelMutation = useMutation<Order, ApiError, number>({
    mutationFn: (orderId) => cancelOrder(orderId, userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ORDERS(userId) });
      showToast('Order cancelled successfully', 'success');
    },
    onError: (error) => {
      showToast(error.message || 'Failed to cancel order', 'error');
    },
  });

  return {
    cancelOrder: cancelMutation.mutateAsync,
    isCancelling: cancelMutation.isPending,
  };
};
