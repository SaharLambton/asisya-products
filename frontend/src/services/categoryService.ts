import api from './api';
import type { Category, CreateCategoryRequest } from '../types';

export const categoryService = {
  getAll: () =>
    api.get<Category[]>('/api/category').then((r) => r.data),

  getById: (id: string) =>
    api.get<Category>(`/api/category/${id}`).then((r) => r.data),

  create: (data: CreateCategoryRequest) =>
    api.post<Category>('/api/category', data).then((r) => r.data),

  delete: (id: string) =>
    api.delete(`/api/category/${id}`),
};
