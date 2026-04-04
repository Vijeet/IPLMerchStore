export type ApiResponse<T = unknown> = {
  success: boolean;
  data?: T;
  message?: string;
  errors?: Record<string, string[]>;
};

export type PaginatedResponse<T> = {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
};

export enum ApiErrorType {
  Network = 'NETWORK_ERROR',
  Validation = 'VALIDATION_ERROR',
  NotFound = 'NOT_FOUND',
  Unauthorized = 'UNAUTHORIZED',
  Forbidden = 'FORBIDDEN',
  ServerError = 'SERVER_ERROR',
  Unknown = 'UNKNOWN_ERROR',
}

export class ApiError extends Error {
  constructor(
    public type: ApiErrorType = ApiErrorType.Unknown,
    public statusCode?: number,
    public details?: Record<string, string[]>
  ) {
    super();
    this.message = type;
  }
}
