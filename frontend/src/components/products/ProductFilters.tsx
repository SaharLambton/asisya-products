import { useState } from 'react';
import type { ProductFilter } from '../../types';
import { useCategories } from '../../hooks/useCategories';
import { inputStyle } from '../common/UI';

interface Props {
  filter: ProductFilter;
  onChange: (f: Partial<ProductFilter>) => void;
}

export function ProductFilters({ filter, onChange }: Props) {
  const { categories } = useCategories();
  const [search, setSearch] = useState(filter.search ?? '');

  const handleSearchKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') onChange({ search, page: 1 });
  };

  return (
    <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', alignItems: 'center', padding: '12px 0' }}>
      <input
        style={{ ...inputStyle, width: 240 }}
        placeholder="🔍  Search by name or description…"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        onKeyDown={handleSearchKeyDown}
        onBlur={() => onChange({ search, page: 1 })}
      />

      <select
        style={{ ...inputStyle, width: 180, background: '#fff' }}
        value={filter.categoryId ?? ''}
        onChange={(e) => onChange({ categoryId: e.target.value || undefined, page: 1 })}
      >
        <option value="">All categories</option>
        {categories.map((c) => (
          <option key={c.id} value={c.id}>{c.name}</option>
        ))}
      </select>

      <select
        style={{ ...inputStyle, width: 140, background: '#fff' }}
        value={filter.isActive === undefined ? '' : String(filter.isActive)}
        onChange={(e) => {
          const v = e.target.value;
          onChange({ isActive: v === '' ? undefined : v === 'true', page: 1 });
        }}
      >
        <option value="">All status</option>
        <option value="true">Active</option>
        <option value="false">Inactive</option>
      </select>

      <select
        style={{ ...inputStyle, width: 120, background: '#fff' }}
        value={filter.pageSize ?? 20}
        onChange={(e) => onChange({ pageSize: Number(e.target.value), page: 1 })}
      >
        {[10, 20, 50, 100].map((n) => (
          <option key={n} value={n}>{n} / page</option>
        ))}
      </select>
    </div>
  );
}
