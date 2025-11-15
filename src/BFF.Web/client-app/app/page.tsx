'use client';

import { useState } from 'react';
import { AuthGuard } from './components/AuthGuard';
import { UserProfile } from './components/UserProfile';
import { useAuth } from './hooks/useAuth';

export default function Home() {
  return (
    <AuthGuard>
      <HomeContent />
    </AuthGuard>
  );
}

function HomeContent() {
  const { user } = useAuth(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  if (!user) return null;

  // -------------------------------------------------------
  // LOAD PRODUCTS MICRO-FRONTEND (via iframe + BFF forward)
  // -------------------------------------------------------
  const loadProductsIframe = () => {
    setError(null);
    setLoading(true);

    const iframe = document.getElementById('microservice-frame') as HTMLIFrameElement;

    if (!iframe) {
      setError('Internal error: iframe not found.');
      setLoading(false);
      return;
    }

    // 🔥 This triggers:
    // Shell BFF → ForwardController → Products BFF → ReceiveLogin → /webUI
    iframe.src = '/bff/forward/products';

    iframe.onload = () => {
      setLoading(false);
    };
  };

  return (
    <div
      style={{
        padding: '2rem',
        fontFamily: 'system-ui, sans-serif',
        maxWidth: '1200px',
        margin: '0 auto',
      }}
    >
      <h1 style={{ marginBottom: '1.5rem' }}>Shell BFF</h1>

      {/* USER PROFILE */}
      <UserProfile claims={user} />

      {/* ERROR BOX */}
      {error && (
        <div
          style={{
            padding: '1rem',
            marginTop: '1rem',
            backgroundColor: '#fee',
            color: '#c00',
            borderRadius: '5px',
            border: '1px solid #fcc',
          }}
        >
          <strong>Error:</strong> {error}
        </div>
      )}

      {/* ONLY ONE BUTTON */}
      <div style={{ marginTop: '2rem' }}>
        <button
          onClick={loadProductsIframe}
          disabled={loading}
          style={{
            padding: '0.75rem 1.5rem',
            backgroundColor: loading ? '#ccc' : '#2563eb',
            color: 'white',
            border: 'none',
            borderRadius: '5px',
            cursor: loading ? 'not-allowed' : 'pointer',
            fontSize: '1rem',
          }}
        >
          {loading ? 'Loading Products UI…' : 'Show Products UI'}
        </button>
      </div>

      {/* IFRAME SECTION */}
      <div style={{ marginTop: '3rem' }}>
        <iframe
          id="microservice-frame"
          style={{
            width: '100%',
            height: '600px',
            border: '1px solid #ccc',
            borderRadius: '5px',
          }}
        />
      </div>

      {/* USER CLAIMS */}
      <div
        style={{
          marginTop: '3rem',
          padding: '1.5rem',
          backgroundColor: '#e7f3ff',
          borderRadius: '8px',
          border: '1px solid #b3d9ff',
        }}
      >
        <h3 style={{ marginTop: 0, color: '#004085' }}>User Claims</h3>

        <ul style={{ margin: 0, paddingLeft: '1.5rem' }}>
          {user.map((claim, index) => (
            <li key={index} style={{ marginBottom: '0.5rem', fontSize: '0.9rem' }}>
              <strong>{claim.type}:</strong>{' '}
              <span style={{ color: '#495057' }}>{claim.value}</span>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}