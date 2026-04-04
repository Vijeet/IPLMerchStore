import { useQuery } from '@tanstack/react-query';
import { ProductDetail } from '@types/product';
import { ApiError } from '@types/common';
import { getProductById as fetchProductById } from '@services/productsApi';
import { QUERY_KEYS } from '@utils/constants';

export const useProductDetails = (id: number | string | undefined) => {
  const numId = typeof id === 'string' ? parseInt(id, 10) : id;

  return useQuery<ProductDetail, ApiError>({
    queryKey: QUERY_KEYS.PRODUCT(numId || 0),
    queryFn: () => {
      if (!numId) {
        throw new Error('Product ID is required');
      }
      return fetchProductById(numId);
    },
    enabled: !!numId, // Only fetch if ID is provided
    staleTime: 10 * 60 * 1000, // 10 minutes
    retry: 1,
  });
};
