import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { authService } from '../services/authService';
import { useAuth } from '../context/AuthContext';
import type { LoginRequest } from '../types';
import { Field, inputStyle, btnPrimary, Alert } from '../components/common/UI';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm<LoginRequest>();

  const onSubmit = async (data: LoginRequest) => {
    setError('');
    setLoading(true);
    try {
      const response = await authService.login(data);
      login(response);
      navigate('/products');
    } catch {
      setError('Invalid username or password. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', background: 'linear-gradient(135deg, #1e3a5f 0%, #2563eb 100%)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 16 }}>
      <div style={{ background: '#fff', borderRadius: 16, padding: 40, width: '100%', maxWidth: 420, boxShadow: '0 25px 60px rgba(0,0,0,0.25)' }}>
        <div style={{ textAlign: 'center', marginBottom: 32 }}>
          <div style={{ fontSize: 40, marginBottom: 8 }}>🗄</div>
          <h1 style={{ margin: '0 0 4px', fontSize: 26, fontWeight: 700, color: '#111827' }}>Asisya Products</h1>
          <p style={{ margin: 0, color: '#6b7280', fontSize: 14 }}>Sign in to manage your inventory</p>
        </div>

        {error && <div style={{ marginBottom: 16 }}><Alert type="error" message={error} /></div>}

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <Field label="Username" error={errors.username?.message}>
            <input
              style={inputStyle}
              placeholder="Enter your username"
              autoComplete="username"
              {...register('username', { required: 'Username is required' })}
            />
          </Field>

          <Field label="Password" error={errors.password?.message}>
            <input
              type="password"
              style={inputStyle}
              placeholder="Enter your password"
              autoComplete="current-password"
              {...register('password', { required: 'Password is required' })}
            />
          </Field>

          <button
            type="submit"
            style={{ ...btnPrimary, width: '100%', padding: '12px', fontSize: 15, marginTop: 8, opacity: loading ? 0.75 : 1 }}
            disabled={loading}
          >
            {loading ? 'Signing in…' : 'Sign In'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: 20, fontSize: 13, color: '#6b7280' }}>
          Don't have an account?{' '}
          <Link to="/register" style={{ color: '#3b82f6', fontWeight: 600, textDecoration: 'none' }}>Register</Link>
        </p>

        <div style={{ marginTop: 24, padding: '12px', background: '#f8fafc', borderRadius: 8, fontSize: 12, color: '#64748b', textAlign: 'center' }}>
          Default admin: <strong>admin</strong> / <strong>Admin@1234!</strong>
        </div>
      </div>
    </div>
  );
}
