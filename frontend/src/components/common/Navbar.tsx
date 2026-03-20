import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav style={{
      background: '#1e3a5f',
      color: '#fff',
      padding: '0 24px',
      height: 60,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      boxShadow: '0 2px 8px rgba(0,0,0,0.2)',
      position: 'sticky',
      top: 0,
      zIndex: 100,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 24 }}>
        <Link to="/products" style={{ color: '#fff', textDecoration: 'none', fontWeight: 700, fontSize: 18, letterSpacing: '-0.5px' }}>
          🗄 Asisya Products
        </Link>
        <Link to="/products" style={{ color: '#93c5fd', textDecoration: 'none', fontSize: 14, fontWeight: 500 }}>
          Products
        </Link>
      </div>
      {user && (
        <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
          <span style={{ fontSize: 13, color: '#cbd5e1' }}>
            👤 <strong style={{ color: '#fff' }}>{user.username}</strong>
            <span style={{ marginLeft: 6, background: '#3b82f6', borderRadius: 99, padding: '2px 8px', fontSize: 11 }}>{user.role}</span>
          </span>
          <button
            onClick={handleLogout}
            style={{ background: 'rgba(255,255,255,0.1)', border: '1px solid rgba(255,255,255,0.2)', color: '#fff', padding: '6px 14px', borderRadius: 6, cursor: 'pointer', fontSize: 13, fontWeight: 500 }}
          >
            Logout
          </button>
        </div>
      )}
    </nav>
  );
}
