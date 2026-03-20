import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { authService } from '../services/authService';
import { useAuth } from '../context/AuthContext';
import type { RegisterRequest } from '../types';
import { Field, inputStyle, btnPrimary, Alert } from '../components/common/UI';

export function RegisterPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const { register, handleSubmit, watch, formState: { errors } } = useForm<RegisterRequest & { confirmPassword: string }>();

  const onSubmit = async (data: RegisterRequest) => {
    setError('');
    setLoading(true);
    try {
      const response = await authService.register(data);
      login(response);
      navigate('/products');
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg ?? 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', background: 'linear-gradient(135deg, #1e3a5f 0%, #2563eb 100%)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 16 }}>
      <div style={{ background: '#fff', borderRadius: 16, padding: 40, width: '100%', maxWidth: 440, boxShadow: '0 25px 60px rgba(0,0,0,0.25)' }}>
        <div style={{ textAlign: 'center', marginBottom: 28 }}>
          <div style={{ fontSize: 36, marginBottom: 8 }}>🗄</div>
          <h1 style={{ margin: '0 0 4px', fontSize: 24, fontWeight: 700, color: '#111827' }}>Create Account</h1>
          <p style={{ margin: 0, color: '#6b7280', fontSize: 14 }}>Join Asisya Products</p>
        </div>

        {error && <div style={{ marginBottom: 16 }}><Alert type="error" message={error} /></div>}

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <Field label="Username" error={errors.username?.message}>
            <input
              style={inputStyle}
              placeholder="Choose a username"
              autoComplete="username"
              {...register('username', {
                required: 'Username is required',
                minLength: { value: 3, message: 'Minimum 3 characters' },
                maxLength: { value: 50, message: 'Maximum 50 characters' },
              })}
            />
          </Field>

          <Field label="Email" error={errors.email?.message}>
            <input
              type="email"
              style={inputStyle}
              placeholder="you@example.com"
              autoComplete="email"
              {...register('email', {
                required: 'Email is required',
                pattern: { value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: 'Invalid email address' },
              })}
            />
          </Field>

          <Field label="Password" error={errors.password?.message}>
            <input
              type="password"
              style={inputStyle}
              placeholder="At least 6 characters"
              autoComplete="new-password"
              {...register('password', {
                required: 'Password is required',
                minLength: { value: 6, message: 'Minimum 6 characters' },
              })}
            />
          </Field>

          <Field label="Confirm Password" error={errors.confirmPassword?.message}>
            <input
              type="password"
              style={inputStyle}
              placeholder="Repeat your password"
              autoComplete="new-password"
              {...register('confirmPassword', {
                required: 'Please confirm your password',
                validate: (val) => val === watch('password') || 'Passwords do not match',
              })}
            />
          </Field>

          <button
            type="submit"
            style={{ ...btnPrimary, width: '100%', padding: '12px', fontSize: 15, marginTop: 4, opacity: loading ? 0.75 : 1 }}
            disabled={loading}
          >
            {loading ? 'Creating account…' : 'Create Account'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: 20, fontSize: 13, color: '#6b7280' }}>
          Already have an account?{' '}
          <Link to="/login" style={{ color: '#3b82f6', fontWeight: 600, textDecoration: 'none' }}>Sign In</Link>
        </p>
      </div>
    </div>
  );
}
