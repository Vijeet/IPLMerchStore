import React from 'react';

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  isLoading?: boolean;
}

/**
 * Pagination component - navigation between pages
 */
export const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  onPageChange,
  isLoading = false,
}) => {
  const pageNumbers = [];
  const maxVisible = 5;
  const halfWindow = Math.floor(maxVisible / 2);

  let startPage = Math.max(1, currentPage - halfWindow);
  let endPage = Math.min(totalPages, startPage + maxVisible - 1);

  if (endPage - startPage + 1 < maxVisible) {
    startPage = Math.max(1, endPage - maxVisible + 1);
  }

  for (let i = startPage; i <= endPage; i++) {
    pageNumbers.push(i);
  }

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        gap: '0.5rem',
        marginTop: '2rem',
        flexWrap: 'wrap',
      }}
    >
      {/* Previous Button */}
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1 || isLoading}
        style={{
          padding: '0.5rem 0.75rem',
          border: '1px solid var(--border-color)',
          borderRadius: 'var(--radius-md)',
          backgroundColor: currentPage === 1 ? 'var(--bg-secondary)' : 'var(--bg-primary)',
          color: 'var(--text-primary)',
          cursor: currentPage === 1 || isLoading ? 'not-allowed' : 'pointer',
          opacity: currentPage === 1 || isLoading ? 0.5 : 1,
          transition: 'all 0.2s',
        }}
      >
        ← Previous
      </button>

      {/* First Page */}
      {startPage > 1 && (
        <>
          <PageButton
            page={1}
            currentPage={currentPage}
            onPageChange={onPageChange}
            isLoading={isLoading}
          />
          {startPage > 2 && (
            <span style={{ color: 'var(--text-secondary)' }}>...</span>
          )}
        </>
      )}

      {/* Page Numbers */}
      {pageNumbers.map((page) => (
        <PageButton
          key={page}
          page={page}
          currentPage={currentPage}
          onPageChange={onPageChange}
          isLoading={isLoading}
        />
      ))}

      {/* Last Page */}
      {endPage < totalPages && (
        <>
          {endPage < totalPages - 1 && (
            <span style={{ color: 'var(--text-secondary)' }}>...</span>
          )}
          <PageButton
            page={totalPages}
            currentPage={currentPage}
            onPageChange={onPageChange}
            isLoading={isLoading}
          />
        </>
      )}

      {/* Next Button */}
      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages || isLoading}
        style={{
          padding: '0.5rem 0.75rem',
          border: '1px solid var(--border-color)',
          borderRadius: 'var(--radius-md)',
          backgroundColor: currentPage === totalPages ? 'var(--bg-secondary)' : 'var(--bg-primary)',
          color: 'var(--text-primary)',
          cursor: currentPage === totalPages || isLoading ? 'not-allowed' : 'pointer',
          opacity: currentPage === totalPages || isLoading ? 0.5 : 1,
          transition: 'all 0.2s',
        }}
      >
        Next →
      </button>

      {/* Page Info */}
      <div
        style={{
          marginLeft: '1rem',
          fontSize: '0.9rem',
          color: 'var(--text-secondary)',
        }}
      >
        Page {currentPage} of {totalPages}
      </div>
    </div>
  );
};

interface PageButtonProps {
  page: number;
  currentPage: number;
  onPageChange: (page: number) => void;
  isLoading?: boolean;
}

const PageButton: React.FC<PageButtonProps> = ({
  page,
  currentPage,
  onPageChange,
  isLoading = false,
}) => {
  const isActive = page === currentPage;

  return (
    <button
      onClick={() => onPageChange(page)}
      disabled={isLoading}
      style={{
        padding: '0.5rem 0.75rem',
        border: isActive ? '2px solid var(--primary-color)' : '1px solid var(--border-color)',
        borderRadius: 'var(--radius-md)',
        backgroundColor: isActive ? 'var(--primary-color)' : 'var(--bg-primary)',
        color: isActive ? 'white' : 'var(--text-primary)',
        fontWeight: isActive ? 600 : 400,
        cursor: isLoading ? 'not-allowed' : 'pointer',
        opacity: isLoading ? 0.6 : 1,
        transition: 'all 0.2s',
        minWidth: '2.5rem',
      }}
    >
      {page}
    </button>
  );
};
