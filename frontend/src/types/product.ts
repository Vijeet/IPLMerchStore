/**
 * Product DTO from list/pagination endpoints
 */
export type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  currency: string; // e.g., "INR"
  imageUrl: string;
  inventoryCount: number;
  productType: number; // 1-8 enum value
  franchiseId: number;
  franchiseName: string;
  isActive: boolean;
  sku: string;
  createdAtUtc: string;
  updatedAtUtc: string;
};

/**
 * Product DTO from detail endpoint
 */
export type ProductDetail = {
  id: number;
  name: string;
  description: string;
  price: number;
  currency: string;
  imageUrl: string;
  inventoryCount: number;
  productType: string; // Display name, e.g., "Jersey"
  franchiseId: number;
  franchiseName: string;
  franchiseShortCode: string;
  isActive: boolean;
  sku: string;
  createdAtUtc: string;
  updatedAtUtc: string;
};

/**
 * Product from search endpoint
 */
export type ProductSearchResult = {
  id: number;
  name: string;
  description: string;
  price: number;
  currency: string;
  imageUrl: string;
  inventoryCount: number;
  productType: number; // Enum value
  productTypeLabel: string; // Display name
  franchiseId: number;
  franchiseName: string;
  isActive: boolean;
  sku: string;
  createdAtUtc: string;
  updatedAtUtc: string;
};

/**
 * Product type enum for filters
 */
export const PRODUCT_TYPES = {
  JERSEY: 1,
  CAP: 2,
  FLAG: 3,
  AUTOGRAPHED_PHOTO: 4,
  MUG: 5,
  HOODIE: 6,
  KEYCHAIN: 7,
  OTHER: 8,
} as const;

export const PRODUCT_TYPE_LABELS: Record<number, string> = {
  1: 'Jersey',
  2: 'Cap',
  3: 'Flag',
  4: 'Autographed Photo',
  5: 'Mug',
  6: 'Hoodie',
  7: 'Keychain',
  8: 'Other',
};

export type CreateProductRequest = {
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  inventoryCount: number;
  franchiseId: number;
  productType: number;
  currency: string;
  sku: string;
};

export type UpdateProductRequest = Partial<CreateProductRequest>;
