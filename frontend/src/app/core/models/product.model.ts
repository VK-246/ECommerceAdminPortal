import { Category } from './category.model';

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// --- Read Models (from API responses) ---

export interface ProductOptionValue {
  id: number;
  value: string;
}

export interface ProductOption {
  id: number;
  attributeId: number;
  name: string; // The attribute name, e.g. "Color"
  values: ProductOptionValue[];
}

export interface ProductVariant {
  id: number;
  sku: string;
  price: number;
  stockQuantity: number;
  optionValues: string[]; // e.g. ["Red", "M"]
  createdAt?: string;
}

export interface PriceRange {
  min: number;
  max: number;
}

export interface Product {
  id: number;
  categoryId: number;
  categoryName?: string;
  name: string;
  description?: string;
  createdAt?: string;
  options: ProductOption[];
  variants: ProductVariant[];
  priceRange?: PriceRange;
  totalStock: number;
}

// --- Write Models (for API requests) ---

export interface CreateOptionValueRequest {
  value: string;
}

export interface CreateOptionRequest {
  attributeId: number;
  values: string[];
}

export interface CreateVariantRequest {
  sku: string;
  price: number;
  stockQuantity: number;
  optionValueIndices: number[]; // [colorIndex, sizeIndex, ...]
}

export interface CreateProductRequest {
  categoryId: number;
  name: string;
  description?: string;
  options: CreateOptionRequest[];
  variants: CreateVariantRequest[];
}

export interface UpdateProductRequest {
  categoryId: number;
  name: string;
  description?: string;
  options: CreateOptionRequest[];
  variants: CreateVariantRequest[];
}

// --- Frontend-only helper for the variant builder ---

export interface VariantRow {
  label: string;         // e.g. "Red / S"
  optionValueIndices: number[];
  sku: string;
  price: number;
  stockQuantity: number;
}
