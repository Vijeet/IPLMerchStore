import { apiClient } from '@services/api';
import { Product, ProductDetail } from '@appTypes/product';
import { PaginatedResponse } from '@appTypes/common';
import { API_ENDPOINTS } from '@utils/constants';

export interface GetProductsParams {
  pageNumber?: number;
  pageSize?: number;
  franchiseId?: number;
  productType?: number;
  activeOnly?: boolean;
  sortBy?: string; // name, name_desc, price, price_desc
}

/**
 * Fetch products with optional filtering and pagination
 */
export const getProducts = async (params: GetProductsParams): Promise<PaginatedResponse<Product>> => {
  const {
    pageNumber = 1,
    pageSize = 12,
    franchiseId,
    productType,
    activeOnly = true,
    sortBy,
  } = params;

  const queryParams = new URLSearchParams();
  queryParams.append('pageNumber', pageNumber.toString());
  queryParams.append('pageSize', pageSize.toString());
  
  if (franchiseId !== undefined) {
    queryParams.append('franchiseId', franchiseId.toString());
  }
  if (productType !== undefined) {
    queryParams.append('productType', productType.toString());
  }
  if (activeOnly !== undefined) {
    queryParams.append('activeOnly', activeOnly.toString());
  }
  if (sortBy) {
    queryParams.append('sortBy', sortBy);
  }

  const response = await apiClient.get<PaginatedResponse<Product>>(
    `${API_ENDPOINTS.PRODUCTS}?${queryParams.toString()}`
  );

  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to fetch products');
  }

  return response.data;
};

/**
 * Fetch a specific product by ID with detailed information
 */
export const getProductById = async (id: number): Promise<ProductDetail> => {
  const response = await apiClient.get<ProductDetail>(
    API_ENDPOINTS.PRODUCT_DETAILS(id)
  );

  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to fetch product details');
  }

  return response.data;
};
