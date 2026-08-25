import { Category } from './category.model';

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface Product {
  id: number;
  categoryId: number;
  categoryName?: string; // Flattened property from backend DTO
  name: string;
  price: number;
  stockQuantity: number;
  description?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateProductRequest {
  categoryId: number;
  name: string;
  price: number;
  stockQuantity: number;
  description?: string;
}

export interface UpdateProductRequest {
  categoryId: number;
  name: string;
  price: number;
  stockQuantity: number;
  description?: string;
}
