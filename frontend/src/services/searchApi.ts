import { apiClient } from '@services/api';
import { ProductSearchResult } from '@types/product';
import { ApiResponse, PaginatedResponse } from '@types/common';
import { API_ENDPOINTS } from '@utils/constants';

export interface SearchProductsParams {
  q?: string; // Query text
  franchiseId?: number;
  type?: number; // Product type 1-8
  page?: number;
  pageSize?: number;
}

/**
 * Search products by query text and optional filters
 * Searches across product names and descriptions
 */
export const searchProducts = async (
  params: SearchProductsParams
): Promise<PaginatedResponse<ProductSearchResult>> => {
  const {
    q,
    franchiseId,
    type,
    page = 1,
    pageSize = 12,
  } = params;

  const queryParams = new URLSearchParams();
  
  if (q) {
    queryParams.append('q', q);
  }
  if (franchiseId !== undefined) {
    queryParams.append('franchiseId', franchiseId.toString());
  }
  if (type !== undefined) {
    queryParams.append('type', type.toString());
  }
  queryParams.append('page', page.toString());
  queryParams.append('pageSize', pageSize.toString());

  const response = await apiClient.get<PaginatedResponse<ProductSearchResult>>(
    `${API_ENDPOINTS.SEARCH}?${queryParams.toString()}`
  );

  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to search products');
  }

  return response.data;
};
