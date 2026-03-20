import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { CreateProductRequest, Product } from '../../types';
import { useCategories } from '../../hooks/useCategories';
import { Field, inputStyle, btnPrimary, btnSecondary } from '../common/UI';

interface ProductFormProps {
  product?: Product;
  onSubmit: (data: CreateProductRequest) => Promise<void>;
  onCancel: () => void;
  loading?: boolean;
}

export function ProductForm({ product, onSubmit, onCancel, loading }: ProductFormProps) {
  const { categories } = useCategories();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateProductRequest>({
    defaultValues: product
      ? {
          name: product.name,
          description: product.description ?? '',
          price: product.price,
          stock: product.stock,
          categoryId: product.categoryId,
        }
      : undefined,
  });

  useEffect(() => {
    if (product) {
      reset({
        name: product.name,
        description: product.description ?? '',
        price: product.price,
        stock: product.stock,
        categoryId: product.categoryId,
      });
    }
  }, [product, reset]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} noValidate>
      <Field label="Name *" error={errors.name?.message}>
        <input
          style={inputStyle}
          placeholder="e.g. Ultra Server 9000"
          {...register('name', {
            required: 'Name is required',
            minLength: { value: 2, message: 'Minimum 2 characters' },
            maxLength: { value: 200, message: 'Maximum 200 characters' },
          })}
        />
      </Field>

      <Field label="Description" error={errors.description?.message}>
        <textarea
          style={{ ...inputStyle, resize: 'vertical', minHeight: 72 }}
          placeholder="Optional description…"
          {...register('description', {
            maxLength: { value: 1000, message: 'Maximum 1000 characters' },
          })}
        />
      </Field>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <Field label="Price (USD) *" error={errors.price?.message}>
          <input
            type="number"
            step="0.01"
            min="0"
            style={inputStyle}
            placeholder="0.00"
            {...register('price', {
              required: 'Price is required',
              min: { value: 0, message: 'Price must be ≥ 0' },
              valueAsNumber: true,
            })}
          />
        </Field>

        <Field label="Stock *" error={errors.stock?.message}>
          <input
            type="number"
            min="0"
            style={inputStyle}
            placeholder="0"
            {...register('stock', {
              required: 'Stock is required',
              min: { value: 0, message: 'Stock must be ≥ 0' },
              valueAsNumber: true,
            })}
          />
        </Field>
      </div>

      <Field label="Category *" error={errors.categoryId?.message}>
        <select
          style={{ ...inputStyle, background: '#fff' }}
          {...register('categoryId', { required: 'Category is required' })}
        >
          <option value="">— Select a category —</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
      </Field>

      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 8 }}>
        <button type="button" style={btnSecondary} onClick={onCancel}>Cancel</button>
        <button type="submit" style={{ ...btnPrimary, opacity: loading ? 0.7 : 1 }} disabled={loading}>
          {loading ? 'Saving…' : product ? 'Save Changes' : 'Create Product'}
        </button>
      </div>
    </form>
  );
}
