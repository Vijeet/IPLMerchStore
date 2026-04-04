import { apiClient } from '@services/api';
import { Franchise } from '@types/franchise';
import { ApiResponse, PaginatedResponse } from '@types/common';
import { API_ENDPOINTS } from '@utils/constants';

/**
 * Fetch all franchises with pagination
 */
export const getFranchises = async (
  pageNumber: number = 1,
  pageSize: number = 100
): Promise<PaginatedResponse<Franchise>> => {
  const response = await apiClient.get<PaginatedResponse<Franchise>>(
    `${API_ENDPOINTS.FRANCHISES}?pageNumber=${pageNumber}&pageSize=${pageSize}`
  );

  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to fetch franchises');
  }

  return response.data;
};

/**
 * Fetch a specific franchise by ID
 */
export const getFranchiseById = async (id: number): Promise<Franchise> => {
  const response = await apiClient.get<Franchise>(
    `${API_ENDPOINTS.FRANCHISES}/${id}`
  );

  if (!response.success || !response.data) {
    throw new Error(response.message || 'Failed to fetch franchise');
  }

  return response.data;
};
