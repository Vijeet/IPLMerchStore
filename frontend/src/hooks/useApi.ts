import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { apiClient } from '@services/api';
import { ApiError } from '@types/common';

/**
 * Generic API hook for GET requests
 */
export const useApi = <T,>(
  queryKey: (string | number)[],
  url: string,
  enabled: boolean = true
): UseQueryResult<T, ApiError> => {
  return useQuery<T, ApiError>({
    queryKey,
    queryFn: async () => {
      const response = await apiClient.get<T>(url);
      return response.data as T;
    },
    enabled,
    retry: 1,
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

/**
 * Hook for fetching a single resource
 */
export const useFetch = <T,>(
  queryKey: (string | number)[],
  url: string,
  enabled: boolean = true
): UseQueryResult<T, ApiError> => {
  return useApi<T>(queryKey, url, enabled);
};

/**
 * Hook for paginated queries
 */
export const usePaginatedApi = <T,>(
  queryKey: (string | number)[],
  url: string,
  pageNumber: number = 1,
  pageSize: number = 20,
  enabled: boolean = true
): UseQueryResult<T, ApiError> => {
  const fullUrl = `${url}?pageNumber=${pageNumber}&pageSize=${pageSize}`;
  return useApi<T>(queryKey, fullUrl, enabled);
};
