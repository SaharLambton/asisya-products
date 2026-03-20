import api from './api';
import type {
  BulkCreateRequest,
  CreateProductRequest,
  PagedResult,
  Product,
  ProductFilter,
  UpdateProductRequest,
} from '../types';

export const productService = {
  getAll: (filter: ProductFilter = {}) =>
    api.get<PagedResult<Product>>('/api/products', { params: filter }).then((r) => r.data),

  getById: (id: string) =>
    api.get<Product>(`/api/products/${id}`).then((r) => r.data),

  create: (data: CreateProductRequest) =>
    api.post<Product>('/api/products', data).then((r) => r.data),

  bulkCreate: (data: BulkCreateRequest) =>
    api.post<{ inserted: number; message: string }>('/api/products/bulk', data).then((r) => r.data),

  update: (id: string, data: UpdateProductRequest) =>
    api.put<Product>(`/api/products/${id}`, data).then((r) => r.data),

  delete: (id: string) =>
    api.delete(`/api/products/${id}`),
};
