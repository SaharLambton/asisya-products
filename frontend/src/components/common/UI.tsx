// ── Spinner ───────────────────────────────────────────────────────────────────
export function Spinner({ size = 'md' }: { size?: 'sm' | 'md' | 'lg' }) {
  const dim = size === 'sm' ? '16px' : size === 'lg' ? '48px' : '32px';
  return (
    <div
      role="status"
      aria-label="Loading"
      style={{
        width: dim,
        height: dim,
        border: '3px solid #e5e7eb',
        borderTopColor: '#3b82f6',
        borderRadius: '50%',
        animation: 'spin 0.7s linear infinite',
        display: 'inline-block',
      }}
    />
  );
}

// ── Alert ─────────────────────────────────────────────────────────────────────
interface AlertProps { type?: 'error' | 'success' | 'info'; message: string }
export function Alert({ type = 'error', message }: AlertProps) {
  const colors = {
    error: { bg: '#fef2f2', border: '#fca5a5', text: '#b91c1c' },
    success: { bg: '#f0fdf4', border: '#86efac', text: '#15803d' },
    info: { bg: '#eff6ff', border: '#93c5fd', text: '#1d4ed8' },
  };
  const c = colors[type];
  return (
    <div style={{ padding: '12px 16px', background: c.bg, border: `1px solid ${c.border}`, borderRadius: 8, color: c.text, fontSize: 14 }}>
      {message}
    </div>
  );
}

// ── Pagination ────────────────────────────────────────────────────────────────
interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (p: number) => void;
}
export function Pagination({ page, totalPages, onPageChange }: PaginationProps) {
  if (totalPages <= 1) return null;
  const btnStyle = (active: boolean, disabled = false): React.CSSProperties => ({
    padding: '6px 12px',
    border: '1px solid #d1d5db',
    borderRadius: 6,
    background: active ? '#3b82f6' : '#fff',
    color: active ? '#fff' : disabled ? '#9ca3af' : '#374151',
    cursor: disabled ? 'not-allowed' : 'pointer',
    fontWeight: active ? 600 : 400,
    fontSize: 14,
  });
  const pages = Array.from({ length: Math.min(totalPages, 7) }, (_, i) => {
    if (totalPages <= 7) return i + 1;
    if (page <= 4) return i + 1;
    if (page >= totalPages - 3) return totalPages - 6 + i;
    return page - 3 + i;
  });
  return (
    <div style={{ display: 'flex', gap: 6, alignItems: 'center', flexWrap: 'wrap' }}>
      <button style={btnStyle(false, page === 1)} disabled={page === 1} onClick={() => onPageChange(page - 1)}>‹ Prev</button>
      {pages.map((p) => (
        <button key={p} style={btnStyle(p === page)} onClick={() => onPageChange(p)}>{p}</button>
      ))}
      <button style={btnStyle(false, page === totalPages)} disabled={page === totalPages} onClick={() => onPageChange(page + 1)}>Next ›</button>
    </div>
  );
}

// ── Modal ─────────────────────────────────────────────────────────────────────
interface ModalProps { title: string; onClose: () => void; children: React.ReactNode }
export function Modal({ title, onClose, children }: ModalProps) {
  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.5)', zIndex: 1000, display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 16 }}>
      <div style={{ background: '#fff', borderRadius: 12, width: '100%', maxWidth: 540, maxHeight: '90vh', overflow: 'auto', boxShadow: '0 20px 60px rgba(0,0,0,0.2)' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '20px 24px', borderBottom: '1px solid #e5e7eb' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 600, color: '#111827' }}>{title}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 22, cursor: 'pointer', color: '#6b7280', lineHeight: 1 }}>×</button>
        </div>
        <div style={{ padding: 24 }}>{children}</div>
      </div>
    </div>
  );
}

// ── Input / Label helpers ─────────────────────────────────────────────────────
export function Field({ label, error, children }: { label: string; error?: string; children: React.ReactNode }) {
  return (
    <div style={{ marginBottom: 16 }}>
      <label style={{ display: 'block', fontSize: 13, fontWeight: 600, color: '#374151', marginBottom: 6 }}>{label}</label>
      {children}
      {error && <p style={{ margin: '4px 0 0', fontSize: 12, color: '#dc2626' }}>{error}</p>}
    </div>
  );
}

export const inputStyle: React.CSSProperties = {
  width: '100%',
  padding: '9px 12px',
  border: '1px solid #d1d5db',
  borderRadius: 8,
  fontSize: 14,
  outline: 'none',
  boxSizing: 'border-box',
  transition: 'border-color 0.15s',
};

export const btnPrimary: React.CSSProperties = {
  padding: '10px 20px',
  background: '#3b82f6',
  color: '#fff',
  border: 'none',
  borderRadius: 8,
  fontWeight: 600,
  fontSize: 14,
  cursor: 'pointer',
};

export const btnDanger: React.CSSProperties = {
  padding: '7px 14px',
  background: '#ef4444',
  color: '#fff',
  border: 'none',
  borderRadius: 6,
  fontWeight: 500,
  fontSize: 13,
  cursor: 'pointer',
};

export const btnSecondary: React.CSSProperties = {
  padding: '7px 14px',
  background: '#f3f4f6',
  color: '#374151',
  border: '1px solid #d1d5db',
  borderRadius: 6,
  fontWeight: 500,
  fontSize: 13,
  cursor: 'pointer',
};
