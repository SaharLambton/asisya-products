import { useState, useEffect, useCallback } from 'react';
import { productService } from '../services/productService';
import type { PagedResult, Product, ProductFilter } from '../types';

interface UseProductsReturn {
  data: PagedResult<Product> | null;
  loading: boolean;
  error: string | null;
  refresh: () => void;
}

export function useProducts(filter: ProductFilter): UseProductsReturn {
  const [data, setData] = useState<PagedResult<Product> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchProducts = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await productService.getAll(filter);
      setData(result);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Failed to load products';
      setError(msg);
    } finally {
      setLoading(false);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filter.page, filter.pageSize, filter.search, filter.categoryId, filter.isActive]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  return { data, loading, error, refresh: fetchProducts };
}
