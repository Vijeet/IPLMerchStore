export const ROUTES = {
  HOME: '/',
  PRODUCTS: '/products',
  PRODUCT_DETAILS: '/products/:id',
  CART: '/cart',
  ORDERS: '/orders',
  NOT_FOUND: '/404',
} as const;

export const API_ENDPOINTS = {
  // Products
  PRODUCTS: '/products',
  PRODUCT_DETAILS: (id: number | string) => `/products/${id}`,
  SEARCH: '/search',

  // Cart
  CART: (userId: string) => `/cart/${userId}`,
  ADD_TO_CART: (userId: string) => `/cart/${userId}/items`,
  UPDATE_CART_ITEM: (userId: string, productId: string) => `/cart/${userId}/items/${productId}`,
  REMOVE_CART_ITEM: (userId: string, productId: string) => `/cart/${userId}/items/${productId}`,
  CLEAR_CART: (userId: string) => `/cart/${userId}`,

  // Orders
  ORDERS: (userId: string) => `/orders/${userId}`,
  ORDER_DETAILS: (userId: string, orderId: string) => `/orders/${userId}/${orderId}`,
  CHECKOUT: (userId: string) => `/orders/checkout?userId=${userId}`,
  CANCEL_ORDER: (userId: string, orderId: string) => `/orders/${userId}/${orderId}/cancel`,

  // Franchises
  FRANCHISES: '/franchises',
} as const;

export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  INTERNAL_SERVER_ERROR: 500,
  SERVICE_UNAVAILABLE: 503,
} as const;

export const QUERY_KEYS = {
  PRODUCTS: ['products'],
  PRODUCTS_SEARCH: (search: string, franchise?: number, type?: number, page?: number) => 
    ['products', 'search', search, franchise, type, page],
  PRODUCT: (id: string | number) => ['product', id],
  FRANCHISES: ['franchises'],
  CART: (userId: string) => ['cart', userId],
  ORDERS: (userId: string) => ['orders', userId],
  ORDER: (userId: string, orderId: string) => ['order', userId, orderId],
} as const;

export const PAGINATION = {
  DEFAULT_PAGE_SIZE: 12,
  MAX_PAGE_SIZE: 100,
  DEFAULT_PAGE_NUMBER: 1,
} as const;

export const ERROR_MESSAGES = {
  NETWORK_ERROR: 'Network error. Please check your connection and try again.',
  SERVER_ERROR: 'Server error. Please try again later.',
  NOT_FOUND: 'Resource not found.',
  UNAUTHORIZED: 'You are not authorized to perform this action.',
  VALIDATION_ERROR: 'Please check your input and try again.',
  UNKNOWN_ERROR: 'An unexpected error occurred. Please try again.',
  PRODUCT_NOT_FOUND: 'Product not found.',
  CART_EMPTY: 'Your cart is empty.',
  INVALID_QUANTITY: 'Invalid quantity. Please enter a valid number.',
  CHECKOUT_FAILED: 'Checkout failed. Please try again.',
  PAYMENT_FAILED: 'Payment failed. Please try again.',
} as const;

export const UI_MESSAGES = {
  LOADING: 'Loading...',
  ADDING_TO_CART: 'Adding to cart...',
  REMOVING_FROM_CART: 'Removing from cart...',
  PROCESSING_CHECKOUT: 'Processing checkout...',
  CART_UPDATED: 'Cart updated successfully!',
  PRODUCT_ADDED: 'Product added to cart!',
  PRODUCT_REMOVED: 'Product removed from cart!',
  ORDER_CREATED: 'Order created successfully!',
} as const;
