import axios, { AxiosInstance, AxiosError } from 'axios';
import { ApiError, ApiErrorType, ApiResponse } from '@types/common';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

class ApiClient {
  private axiosInstance: AxiosInstance;

  constructor(baseURL: string = API_BASE_URL) {
    this.axiosInstance = axios.create({
      baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add response interceptor for error handling
    this.axiosInstance.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        const apiError = this.transformError(error);
        return Promise.reject(apiError);
      }
    );
  }

  private transformError(error: AxiosError): ApiError {
    if (!error.response) {
      return new ApiError(ApiErrorType.Network, undefined, undefined);
    }

    const status = error.response.status;
    const data = error.response.data as Record<string, unknown>;

    switch (status) {
      case 400:
        return new ApiError(
          ApiErrorType.Validation,
          status,
          (data.errors as Record<string, string[]>) || undefined
        );
      case 401:
        return new ApiError(ApiErrorType.Unauthorized, status);
      case 403:
        return new ApiError(ApiErrorType.Forbidden, status);
      case 404:
        return new ApiError(ApiErrorType.NotFound, status);
      case 500:
      case 502:
      case 503:
      case 504:
        return new ApiError(ApiErrorType.ServerError, status);
      default:
        return new ApiError(ApiErrorType.Unknown, status);
    }
  }

  async get<T>(url: string): Promise<ApiResponse<T>> {
    const response = await this.axiosInstance.get<ApiResponse<T>>(url);
    return response.data;
  }

  async post<T>(url: string, data?: unknown): Promise<ApiResponse<T>> {
    const response = await this.axiosInstance.post<ApiResponse<T>>(url, data);
    return response.data;
  }

  async put<T>(url: string, data?: unknown): Promise<ApiResponse<T>> {
    const response = await this.axiosInstance.put<ApiResponse<T>>(url, data);
    return response.data;
  }

  async patch<T>(url: string, data?: unknown): Promise<ApiResponse<T>> {
    const response = await this.axiosInstance.patch<ApiResponse<T>>(url, data);
    return response.data;
  }

  async delete<T>(url: string): Promise<ApiResponse<T>> {
    const response = await this.axiosInstance.delete<ApiResponse<T>>(url);
    return response.data;
  }
}

export const apiClient = new ApiClient();
