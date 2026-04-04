import { useQuery } from '@tanstack/react-query';
import { Product } from '@types/product';
import { PaginatedResponse } from '@types/common';
import { ApiError } from '@types/common';
import { getProducts as fetchProducts, GetProductsParams } from '@services/productsApi';
import { QUERY_KEYS } from '@utils/constants';

export const useProducts = (params: GetProductsParams = {}) => {
  const {
    pageNumber = 1,
    pageSize = 12,
    franchiseId,
    productType,
    activeOnly = true,
    sortBy,
  } = params;

  const queryKey = [
    ...QUERY_KEYS.PRODUCTS,
    pageNumber,
    pageSize,
    franchiseId,
    productType,
    sortBy,
  ];

  return useQuery<PaginatedResponse<Product>, ApiError>({
    queryKey,
    queryFn: () =>
      fetchProducts({
        pageNumber,
        pageSize,
        franchiseId,
        productType,
        activeOnly,
        sortBy,
      }),
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: 1,
  });
};
