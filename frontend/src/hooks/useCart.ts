import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Cart, AddToCartRequest, UpdateCartItemRequest } from '@appTypes/cart';
import { ApiError } from '@appTypes/common';
import { getCart, addToCart, updateCartItem, removeCartItem } from '@services/cartApi';
import { QUERY_KEYS } from '@utils/constants';

const DEMO_USER_ID = 'demo-user-1';

export const useCart = (userId: string = DEMO_USER_ID) => {
  const queryClient = useQueryClient();
  const cartQueryKey = QUERY_KEYS.CART(userId);

  const cartQuery = useQuery<Cart, ApiError>({
    queryKey: cartQueryKey,
    queryFn: () => getCart(userId),
    retry: 1,
    staleTime: 2 * 60 * 1000,
  });

  const addMutation = useMutation<Cart, ApiError, AddToCartRequest>({
    mutationFn: (request) => addToCart(request, userId),
    onSuccess: (data) => {
      queryClient.setQueryData(cartQueryKey, data);
    },
  });

  const updateMutation = useMutation<
    Cart,
    ApiError,
    { productId: number; request: UpdateCartItemRequest }
  >({
    mutationFn: ({ productId, request }) =>
      updateCartItem(productId, request, userId),
    onSuccess: (data) => {
      queryClient.setQueryData(cartQueryKey, data);
    },
  });

  const removeMutation = useMutation<Cart, ApiError, number>({
    mutationFn: (productId) => removeCartItem(productId, userId),
    onSuccess: (data) => {
      queryClient.setQueryData(cartQueryKey, data);
    },
  });

  const isMutating =
    addMutation.isPending ||
    updateMutation.isPending ||
    removeMutation.isPending;

  return {
    cart: cartQuery.data,
    isLoading: cartQuery.isLoading,
    isError: cartQuery.isError,
    error: cartQuery.error,

    addToCart: addMutation.mutateAsync,
    updateItem: (productId: number, quantity: number) =>
      updateMutation.mutateAsync({ productId, request: { quantity } }),
    removeItem: removeMutation.mutateAsync,

    isMutating,
    addError: addMutation.error,
    updateError: updateMutation.error,
    removeError: removeMutation.error,
  };
};
