import { useQuery } from '@tanstack/react-query';
import { ApiError } from '@appTypes/common';
import { getFranchises as fetchFranchises } from '@services/franchisesApi';
import { QUERY_KEYS, PAGINATION } from '@utils/constants';

export const useFranchises = (pageSize: number = PAGINATION.DEFAULT_PAGE_SIZE) => {
  return useQuery<any, ApiError>({
    queryKey: QUERY_KEYS.FRANCHISES,
    queryFn: async () => {
      const result = await fetchFranchises(1, pageSize);
      return result.items; // Return just the items array for simplicity
    },
    staleTime: 10 * 60 * 1000, // 10 minutes
    retry: 1,
  });
};
