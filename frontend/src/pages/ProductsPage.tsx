import { useState } from 'react';
import { productService } from '../services/productService';
import { useProducts } from '../hooks/useProducts';
import { useCategories } from '../hooks/useCategories';
import { ProductForm } from '../components/products/ProductForm';
import { ProductFilters } from '../components/products/ProductFilters';
import {
  Spinner, Alert, Pagination, Modal,
  btnPrimary, btnDanger, btnSecondary,
  Field, inputStyle,
} from '../components/common/UI';
import type { CreateProductRequest, Product, ProductFilter } from '../types';

const BADGE_COLORS: Record<string, string> = {
  SERVIDORES: '#1d4ed8',
  CLOUD: '#7c3aed',
};

function categoryBadge(name: string | null) {
  const bg = BADGE_COLORS[name ?? ''] ?? '#374151';
  return (
    <span style={{ background: bg, color: '#fff', padding: '2px 8px', borderRadius: 99, fontSize: 11, fontWeight: 600 }}>
      {name ?? '—'}
    </span>
  );
}

export function ProductsPage() {
  const [filter, setFilter] = useState<ProductFilter>({ page: 1, pageSize: 20 });
  const { data, loading, error, refresh } = useProducts(filter);
  const { categories } = useCategories();

  // Modals
  const [showCreate, setShowCreate] = useState(false);
  const [editProduct, setEditProduct] = useState<Product | null>(null);
  const [deleteProduct, setDeleteProduct] = useState<Product | null>(null);
  const [showBulk, setShowBulk] = useState(false);

  // Mutation state
  const [mutating, setMutating] = useState(false);
  const [mutationError, setMutationError] = useState('');
  const [bulkCount, setBulkCount] = useState(1000);
  const [bulkCategoryId, setBulkCategoryId] = useState('');
  const [bulkResult, setBulkResult] = useState('');

  const mergeFilter = (patch: Partial<ProductFilter>) =>
    setFilter((prev) => ({ ...prev, ...patch }));

  // ── Create ──────────────────────────────────────────────────────────────────
  const handleCreate = async (dto: CreateProductRequest) => {
    setMutating(true);
    setMutationError('');
    try {
      await productService.create(dto);
      setShowCreate(false);
      refresh();
    } catch {
      setMutationError('Failed to create product.');
    } finally {
      setMutating(false);
    }
  };

  // ── Edit ────────────────────────────────────────────────────────────────────
  const handleEdit = async (dto: CreateProductRequest) => {
    if (!editProduct) return;
    setMutating(true);
    setMutationError('');
    try {
      await productService.update(editProduct.id, dto);
      setEditProduct(null);
      refresh();
    } catch {
      setMutationError('Failed to update product.');
    } finally {
      setMutating(false);
    }
  };

  // ── Delete ──────────────────────────────────────────────────────────────────
  const handleDelete = async () => {
    if (!deleteProduct) return;
    setMutating(true);
    try {
      await productService.delete(deleteProduct.id);
      setDeleteProduct(null);
      refresh();
    } catch {
      setMutationError('Failed to delete product.');
    } finally {
      setMutating(false);
    }
  };

  // ── Bulk ────────────────────────────────────────────────────────────────────
  const handleBulk = async () => {
    setMutating(true);
    setBulkResult('');
    setMutationError('');
    try {
      const res = await productService.bulkCreate({
        count: bulkCount,
        categoryId: bulkCategoryId || undefined,
      });
      setBulkResult(res.message);
      refresh();
    } catch {
      setMutationError('Bulk insert failed. Check count and category.');
    } finally {
      setMutating(false);
    }
  };

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '24px 16px' }}>
      {/* ── Header ────────────────────────────────────────────────────────── */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: 12, marginBottom: 8 }}>
        <div>
          <h1 style={{ margin: 0, fontSize: 26, fontWeight: 700, color: '#111827' }}>Products</h1>
          {data && (
            <p style={{ margin: '4px 0 0', color: '#6b7280', fontSize: 14 }}>
              {data.totalCount.toLocaleString()} total products
            </p>
          )}
        </div>
        <div style={{ display: 'flex', gap: 10 }}>
          <button style={btnSecondary} onClick={() => { setShowBulk(true); setBulkResult(''); setMutationError(''); }}>
            ⚡ Bulk Insert
          </button>
          <button style={btnPrimary} onClick={() => { setShowCreate(true); setMutationError(''); }}>
            + New Product
          </button>
        </div>
      </div>

      {/* ── Filters ───────────────────────────────────────────────────────── */}
      <ProductFilters filter={filter} onChange={mergeFilter} />

      {/* ── Content ───────────────────────────────────────────────────────── */}
      {error && <Alert type="error" message={error} />}

      {loading ? (
        <div style={{ display: 'flex', justifyContent: 'center', padding: 60 }}><Spinner size="lg" /></div>
      ) : (
        <>
          <div style={{ overflowX: 'auto', borderRadius: 10, border: '1px solid #e5e7eb', background: '#fff', boxShadow: '0 1px 4px rgba(0,0,0,0.06)' }}>
            <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 14 }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '1px solid #e5e7eb' }}>
                  {['Name', 'Category', 'Price', 'Stock', 'Status', 'Actions'].map((h) => (
                    <th key={h} style={{ padding: '12px 16px', textAlign: 'left', fontWeight: 600, color: '#374151', whiteSpace: 'nowrap' }}>{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {data?.items.length === 0 && (
                  <tr>
                    <td colSpan={6} style={{ textAlign: 'center', padding: 48, color: '#9ca3af' }}>
                      No products found. Try adjusting your filters.
                    </td>
                  </tr>
                )}
                {data?.items.map((p, i) => (
                  <tr
                    key={p.id}
                    style={{ borderBottom: '1px solid #f3f4f6', background: i % 2 === 0 ? '#fff' : '#fafafa', transition: 'background 0.1s' }}
                  >
                    <td style={{ padding: '12px 16px' }}>
                      <div style={{ fontWeight: 600, color: '#111827', maxWidth: 220, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{p.name}</div>
                      {p.description && (
                        <div style={{ fontSize: 12, color: '#9ca3af', marginTop: 2, maxWidth: 220, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{p.description}</div>
                      )}
                    </td>
                    <td style={{ padding: '12px 16px' }}>
                      <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                        {p.categoryImageUrl && (
                          <img src={p.categoryImageUrl} alt={p.categoryName ?? ''} style={{ width: 20, height: 20, borderRadius: 4, objectFit: 'cover' }} />
                        )}
                        {categoryBadge(p.categoryName)}
                      </div>
                    </td>
                    <td style={{ padding: '12px 16px', fontWeight: 600, color: '#059669' }}>
                      ${p.price.toLocaleString('en-US', { minimumFractionDigits: 2 })}
                    </td>
                    <td style={{ padding: '12px 16px', color: p.stock === 0 ? '#ef4444' : '#374151' }}>
                      {p.stock.toLocaleString()}
                    </td>
                    <td style={{ padding: '12px 16px' }}>
                      <span style={{
                        padding: '3px 10px', borderRadius: 99, fontSize: 12, fontWeight: 600,
                        background: p.isActive ? '#dcfce7' : '#fee2e2',
                        color: p.isActive ? '#15803d' : '#b91c1c',
                      }}>
                        {p.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td style={{ padding: '12px 16px' }}>
                      <div style={{ display: 'flex', gap: 6 }}>
                        <button
                          style={{ ...btnSecondary, padding: '5px 12px', fontSize: 12 }}
                          onClick={() => { setEditProduct(p); setMutationError(''); }}
                        >
                          ✏️ Edit
                        </button>
                        <button
                          style={{ ...btnDanger, padding: '5px 12px', fontSize: 12 }}
                          onClick={() => setDeleteProduct(p)}
                        >
                          🗑 Delete
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* ── Pagination ─────────────────────────────────────────────────── */}
          {data && data.totalPages > 1 && (
            <div style={{ marginTop: 20, display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 8 }}>
              <span style={{ fontSize: 13, color: '#6b7280' }}>
                Showing {((filter.page! - 1) * filter.pageSize!) + 1}–{Math.min(filter.page! * filter.pageSize!, data.totalCount)} of {data.totalCount.toLocaleString()}
              </span>
              <Pagination page={data.page} totalPages={data.totalPages} onPageChange={(p) => mergeFilter({ page: p })} />
            </div>
          )}
        </>
      )}

      {/* ── Create Modal ──────────────────────────────────────────────────── */}
      {showCreate && (
        <Modal title="Create Product" onClose={() => setShowCreate(false)}>
          {mutationError && <div style={{ marginBottom: 16 }}><Alert type="error" message={mutationError} /></div>}
          <ProductForm onSubmit={handleCreate} onCancel={() => setShowCreate(false)} loading={mutating} />
        </Modal>
      )}

      {/* ── Edit Modal ────────────────────────────────────────────────────── */}
      {editProduct && (
        <Modal title="Edit Product" onClose={() => setEditProduct(null)}>
          {mutationError && <div style={{ marginBottom: 16 }}><Alert type="error" message={mutationError} /></div>}
          <ProductForm product={editProduct} onSubmit={handleEdit} onCancel={() => setEditProduct(null)} loading={mutating} />
        </Modal>
      )}

      {/* ── Delete Confirm Modal ──────────────────────────────────────────── */}
      {deleteProduct && (
        <Modal title="Confirm Delete" onClose={() => setDeleteProduct(null)}>
          <p style={{ margin: '0 0 20px', color: '#374151' }}>
            Are you sure you want to delete <strong>"{deleteProduct.name}"</strong>? This action cannot be undone.
          </p>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
            <button style={btnSecondary} onClick={() => setDeleteProduct(null)}>Cancel</button>
            <button style={{ ...btnDanger, opacity: mutating ? 0.7 : 1 }} onClick={handleDelete} disabled={mutating}>
              {mutating ? 'Deleting…' : 'Delete'}
            </button>
          </div>
        </Modal>
      )}

      {/* ── Bulk Insert Modal ────────────────────────────────────────────── */}
      {showBulk && (
        <Modal title="⚡ Bulk Insert Products" onClose={() => setShowBulk(false)}>
          <p style={{ margin: '0 0 16px', color: '#6b7280', fontSize: 14 }}>
            Generate and insert a large number of random products. The API handles up to 100,000 at once using batch inserts.
          </p>
          {mutationError && <div style={{ marginBottom: 12 }}><Alert type="error" message={mutationError} /></div>}
          {bulkResult && <div style={{ marginBottom: 12 }}><Alert type="success" message={bulkResult} /></div>}
          <Field label="Number of products (1 – 100,000)">
            <input
              type="number"
              min={1}
              max={100000}
              style={inputStyle}
              value={bulkCount}
              onChange={(e) => setBulkCount(Number(e.target.value))}
            />
          </Field>
          <Field label="Category (optional — random if empty)">
            <select
              style={{ ...inputStyle, background: '#fff' }}
              value={bulkCategoryId}
              onChange={(e) => setBulkCategoryId(e.target.value)}
            >
              <option value="">— Random category —</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </Field>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
            <button style={btnSecondary} onClick={() => setShowBulk(false)}>Close</button>
            <button
              style={{ ...btnPrimary, opacity: mutating ? 0.7 : 1, background: '#7c3aed' }}
              onClick={handleBulk}
              disabled={mutating}
            >
              {mutating ? `Inserting ${bulkCount.toLocaleString()}…` : `Insert ${bulkCount.toLocaleString()} products`}
            </button>
          </div>
        </Modal>
      )}
    </div>
  );
}
