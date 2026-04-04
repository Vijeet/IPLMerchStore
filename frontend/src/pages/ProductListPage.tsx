import React, { useMemo } from 'react';
import { useSearchParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { LoadingSpinner, EmptyState, ErrorBoundary } from '@components/shared';
import { SearchBar } from '@components/catalog/SearchBar';
import { FranchiseFilter } from '@components/catalog/FranchiseFilter';
import { ProductTypeFilter } from '@components/catalog/ProductTypeFilter';
import { ProductCard } from '@components/catalog/ProductCard';
import { Pagination } from '@components/catalog/Pagination';
import { useFranchises } from '@hooks/useFranchises';
import { searchProducts } from '@services/searchApi';
import { getProducts } from '@services/productsApi';
import { Product, ProductSearchResult } from '@appTypes/product';
import { PAGINATION } from '@utils/constants';

export const ProductListPage: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams();

  // Parse URL parameters
  const searchQuery = searchParams.get('search') || '';
  const franchiseParam = searchParams.get('franchise');
  const typeParam = searchParams.get('type');
  const pageParam = searchParams.get('page') || '1';

  const franchiseId = franchiseParam ? parseInt(franchiseParam, 10) : undefined;
  const productType = typeParam ? parseInt(typeParam, 10) : undefined;
  const currentPage = Math.max(1, parseInt(pageParam, 10) || 1);

  // Fetch franchises for filter
  const { data: franchises = [], isLoading: franchisesLoading } = useFranchises();

  // Determine which API to call based on search query
  const shouldUseSearch = searchQuery.trim().length > 0;

  // Fetch products or search results
  const { data: productsData = null, isLoading: productsLoading, error: productsError } = useQuery({
    queryKey: [
      shouldUseSearch ? 'search' : 'products',
      searchQuery,
      franchiseId,
      productType,
      currentPage,
    ],
    queryFn: async () => {
      if (shouldUseSearch) {
        return searchProducts({
          q: searchQuery,
          franchiseId,
          type: productType,
          page: currentPage,
          pageSize: PAGINATION.DEFAULT_PAGE_SIZE,
        });
      } else {
        return getProducts({
          pageNumber: currentPage,
          pageSize: PAGINATION.DEFAULT_PAGE_SIZE,
          franchiseId,
          productType,
          activeOnly: true,
        });
      }
    },
  });

  // Handle all possible product types
  const products = useMemo(() => {
    if (!productsData) return [];
    return productsData.items as (Product | ProductSearchResult)[];
  }, [productsData]);

  // Update URL parameters - use setSearchParams callback form to always get fresh params
  const updateFilter = React.useCallback(
    (updates: Record<string, string | number | undefined>) => {
      setSearchParams((currentParams) => {
        const newParams = new URLSearchParams(currentParams);

        Object.entries(updates).forEach(([key, value]) => {
          if (value === undefined || value === '') {
            newParams.delete(key);
          } else {
            newParams.set(key, String(value));
          }
        });

        // Reset to page 1 when filters change (not when page is updated alone)
        if (Object.keys(updates).some((k) => k !== 'page')) {
          newParams.set('page', '1');
        }

        return newParams;
      });
    },
    [setSearchParams]
  );

  const handleSearchChange = React.useCallback(
    (value: string) => {
      updateFilter({ search: value });
    },
    [updateFilter]
  );

  const handleFranchiseChange = React.useCallback(
    (id: number | undefined) => {
      updateFilter({ franchise: id });
    },
    [updateFilter]
  );

  const handleTypeChange = React.useCallback(
    (type: number | undefined) => {
      updateFilter({ type });
    },
    [updateFilter]
  );

  const handlePageChange = React.useCallback(
    (page: number) => {
      updateFilter({ page });
    },
    [updateFilter]
  );

  const isLoading = productsLoading;
  const totalPages = productsData?.totalPages || 1;

  return (
    <div style={{ padding: '2rem 0' }}>
      {/* Header */}
      <div style={{ marginBottom: '2rem' }}>
        <h1 style={{ fontSize: '2rem', fontWeight: 700, marginBottom: '0.5rem' }}>
          Products
        </h1>
        <p style={{ color: 'var(--text-secondary)', marginBottom: '1.5rem' }}>
          Browse our collection of merchandise
        </p>
      </div>

      {/* Filters Section */}
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
          gap: '1.5rem',
          marginBottom: '2rem',
          backgroundColor: 'var(--bg-secondary)',
          padding: '1.5rem',
          borderRadius: 'var(--radius-lg)',
        }}
      >
        <SearchBar
          value={searchQuery}
          onChange={handleSearchChange}
          placeholder="Search products by name or description..."
        />

        <FranchiseFilter
          franchises={franchises}
          selectedFranchiseId={franchiseId}
          onChange={handleFranchiseChange}
          isLoading={franchisesLoading}
        />

        <ProductTypeFilter
          selectedType={productType}
          onChange={handleTypeChange}
        />
      </div>

      {/* Loading State */}
      {isLoading && <LoadingSpinner message="Loading products..." />}

      {/* Error State */}
      {productsError && (
        <ErrorBoundary
          error={productsError}
          onRetry={() => {
            window.location.reload();
          }}
        />
      )}

      {/* Products Grid */}
      {!isLoading && !productsError && products.length > 0 && (
        <>
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))',
              gap: '1.5rem',
              marginBottom: '2rem',
            }}
          >
            {products.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={handlePageChange}
              isLoading={isLoading}
            />
          )}
        </>
      )}

      {/* Empty State */}
      {!isLoading && !productsError && products.length === 0 && (
        <EmptyState
          title="No Products Found"
          description={
            searchQuery
              ? `No products match your search for "${searchQuery}". Try adjusting your filters.`
              : 'No products available. Check back soon!'
          }
          icon="📦"
        />
      )}
    </div>
  );
};
